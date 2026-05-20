// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MTConnect.NET_DocsGen;

/// <summary>
/// A single environment-variable read site discovered by Roslyn.
/// </summary>
public sealed record EnvVarInfo(
    string Name,
    string FallbackLiteral,
    string FileRelativePath,
    int Line,
    string Context);

/// <summary>
/// Walks every `*.cs` file under the repository (excluding build
/// outputs and the docs/ node_modules tree) and records every site
/// that reads an environment variable. The current pattern set:
///
///   - `Environment.GetEnvironmentVariable("NAME")`
///   - `Environment.GetEnvironmentVariable("NAME", EnvironmentVariableTarget.X)`
///
/// Each call captures the string literal name, the optional default-
/// fallback literal that appears on the same line, the file:line
/// location, and a short snippet of context.
/// </summary>
public static class EnvVarInventory
{
    private static readonly string[] ExcludedDirectoryNames =
    {
        "bin", "obj", "node_modules", ".git", ".vitepress",
    };

    public static IReadOnlyList<EnvVarInfo> Collect(string repoRoot)
    {
        var results = new List<EnvVarInfo>();

        foreach (var file in EnumerateCsFiles(repoRoot))
        {
            string text;
            try { text = File.ReadAllText(file); }
            catch { continue; }

            // Cheap text gate before parsing.
            if (!text.Contains("GetEnvironmentVariable", StringComparison.Ordinal)) continue;

            var tree = CSharpSyntaxTree.ParseText(text, path: file);
            var rel = Path.GetRelativePath(repoRoot, file).Replace('\\', '/');

            // Local resolution of `const string X = "..."` so call sites
            // like `GetEnvironmentVariable(FixtureDirEnv)` still resolve
            // without a semantic model.
            var localConsts = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (var field in tree.GetRoot().DescendantNodes().OfType<FieldDeclarationSyntax>())
            {
                bool isConst = field.Modifiers.Any(t => t.IsKind(SyntaxKind.ConstKeyword));
                if (!isConst) continue;
                foreach (var v in field.Declaration.Variables)
                {
                    if (v.Initializer?.Value is LiteralExpressionSyntax l
                        && l.IsKind(SyntaxKind.StringLiteralExpression))
                    {
                        localConsts[v.Identifier.ValueText] = l.Token.ValueText;
                    }
                }
            }

            foreach (var inv in tree.GetRoot().DescendantNodes().OfType<InvocationExpressionSyntax>())
            {
                if (inv.Expression is not MemberAccessExpressionSyntax m) continue;
                if (m.Name.Identifier.ValueText != "GetEnvironmentVariable") continue;

                var args = inv.ArgumentList.Arguments;
                if (args.Count == 0) continue;
                string? name = null;
                if (args[0].Expression is LiteralExpressionSyntax lit && lit.IsKind(SyntaxKind.StringLiteralExpression))
                {
                    name = lit.Token.ValueText;
                }
                else if (args[0].Expression is IdentifierNameSyntax id
                         && localConsts.TryGetValue(id.Identifier.ValueText, out var resolved))
                {
                    name = resolved;
                }
                if (name is null) continue;

                var line = inv.GetLocation().GetLineSpan().StartLinePosition.Line + 1;

                // Look at the enclosing statement / expression for a
                // ?? "default" or `?? Environment.GetEnvironmentVariable(...)`
                // fallback.
                var fallback = string.Empty;
                var parent = inv.Parent;
                while (parent is not null && parent is not StatementSyntax && parent is not MemberDeclarationSyntax)
                {
                    if (parent is BinaryExpressionSyntax bin && bin.IsKind(SyntaxKind.CoalesceExpression))
                    {
                        fallback = bin.Right.ToString();
                        break;
                    }
                    parent = parent.Parent;
                }

                var context = inv.Parent?.ToString() ?? inv.ToString();
                if (context.Length > 160) context = context.Substring(0, 157) + "...";
                context = context.Replace('\n', ' ').Replace('\r', ' ').Replace('|', '/');

                results.Add(new EnvVarInfo(name, fallback, rel, line, context));
            }
        }

        return results
            .OrderBy(e => e.Name, StringComparer.Ordinal)
            .ThenBy(e => e.FileRelativePath, StringComparer.Ordinal)
            .ThenBy(e => e.Line)
            .ToList();
    }

    private static IEnumerable<string> EnumerateCsFiles(string root)
    {
        var stack = new Stack<string>();
        stack.Push(root);
        while (stack.Count > 0)
        {
            var dir = stack.Pop();
            IEnumerable<string> subs;
            try { subs = Directory.EnumerateDirectories(dir); }
            catch { continue; }
            foreach (var sub in subs)
            {
                var name = Path.GetFileName(sub);
                if (ExcludedDirectoryNames.Contains(name, StringComparer.Ordinal)) continue;
                stack.Push(sub);
            }
            IEnumerable<string> files;
            try { files = Directory.EnumerateFiles(dir, "*.cs"); }
            catch { continue; }
            foreach (var f in files) yield return f;
        }
    }
}
