// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;

namespace MTConnect.NET_DocsGen;

/// <summary>
/// One discovered configuration option class with its inventory of
/// settable / writable properties. The schema is derived from the
/// source-level property declarations, the YAML / JSON attribute
/// aliases, and the `///` summary above each property.
/// </summary>
public sealed record ConfigClassInfo(
    string TypeName,
    string Namespace,
    string FileRelativePath,
    string Summary,
    IReadOnlyList<ConfigPropertyInfo> Properties);

/// <summary>
/// A single configuration property.
/// </summary>
public sealed record ConfigPropertyInfo(
    string Name,
    string Type,
    string SerialisedKey,
    string Summary,
    string? DefaultLiteral);

/// <summary>
/// Builds the configuration inventory. Limited deliberately to the
/// agent / adapter / module configuration surface — wire-format
/// "configurations" (sensor configuration, namespace configuration,
/// etc.) are already documented in DocFX-rendered API reference
/// pages and would only add noise here.
/// </summary>
public static class ConfigInventory
{
    /// <summary>
    /// Files whose stem appears here are treated as agent / adapter
    /// configuration entry points and walked for property options.
    /// Anything else under `libraries/*/Configurations/` is skipped
    /// (e.g. namespace / style / device-level wire configuration).
    /// </summary>
    private static readonly HashSet<string> IncludedStems = new(StringComparer.Ordinal)
    {
        "AgentConfiguration",
        "AgentApplicationConfiguration",
        "AdapterApplicationConfiguration",
        "IAgentConfiguration",
        "IAgentApplicationConfiguration",
        "IAdapterApplicationConfiguration",
        "DataSourceConfiguration",
        "IDataSourceConfiguration",
        "HttpClientConfiguration",
        "IHttpClientConfiguration",
        "ShdrClientConfiguration",
        "IShdrClientConfiguration",
        "ShdrAdapterClientConfiguration",
        "IShdrAdapterClientConfiguration",
        "TlsConfiguration",
        "PfxCertificateConfiguration",
        "PemCertificateConfiguration",
        "FileConfiguration",
        "IFileConfiguration",
    };

    public static IReadOnlyList<ConfigClassInfo> Collect(string repoRoot)
    {
        var results = new List<ConfigClassInfo>();

        var librariesDir = Path.Combine(repoRoot, "libraries");
        if (!Directory.Exists(librariesDir)) return results;

        foreach (var file in Directory.EnumerateFiles(librariesDir, "*.cs", SearchOption.AllDirectories))
        {
            // Skip generated and build outputs.
            if (file.Contains("/bin/", StringComparison.Ordinal)
                || file.Contains("/obj/", StringComparison.Ordinal)
                || file.Contains("\\bin\\", StringComparison.Ordinal)
                || file.Contains("\\obj\\", StringComparison.Ordinal)) continue;
            if (file.EndsWith(".g.cs", StringComparison.Ordinal)) continue;

            var stem = Path.GetFileNameWithoutExtension(file);
            if (!IncludedStems.Contains(stem)) continue;

            var text = File.ReadAllText(file);
            var rel = Path.GetRelativePath(repoRoot, file).Replace('\\', '/');
            var tree = CSharpSyntaxTree.ParseText(text, path: file);

            foreach (var typeDecl in tree.GetRoot().DescendantNodes().OfType<BaseTypeDeclarationSyntax>())
            {
                if (typeDecl is not (ClassDeclarationSyntax or InterfaceDeclarationSyntax)) continue;
                if (typeDecl.Identifier.ValueText != stem) continue;

                var ns = (typeDecl.Ancestors().OfType<BaseNamespaceDeclarationSyntax>().FirstOrDefault()?
                    .Name.ToString()) ?? string.Empty;

                var typeSummary = ExtractSummary(typeDecl.GetLeadingTrivia());

                var props = new List<ConfigPropertyInfo>();
                foreach (var member in typeDecl.ChildNodes().OfType<PropertyDeclarationSyntax>())
                {
                    // Public, instance properties only.
                    bool isPublic = member.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword));
                    bool isInterface = typeDecl is InterfaceDeclarationSyntax;
                    if (!isPublic && !isInterface) continue;

                    // Skip [JsonIgnore]/[YamlIgnore] both-ignored properties.
                    bool jsonIgnore = HasAttr(member.AttributeLists, "JsonIgnore");
                    bool yamlIgnore = HasAttr(member.AttributeLists, "YamlIgnore");
                    if (jsonIgnore && yamlIgnore) continue;

                    var summary = ExtractSummary(member.GetLeadingTrivia());
                    var serialisedKey = ExtractSerialisedKey(member.AttributeLists, member.Identifier.ValueText);
                    var type = member.Type.ToString();

                    props.Add(new ConfigPropertyInfo(
                        Name: member.Identifier.ValueText,
                        Type: type,
                        SerialisedKey: serialisedKey,
                        Summary: summary,
                        DefaultLiteral: null));
                }

                if (props.Count == 0) continue;

                results.Add(new ConfigClassInfo(
                    TypeName: typeDecl.Identifier.ValueText,
                    Namespace: ns,
                    FileRelativePath: rel,
                    Summary: typeSummary,
                    Properties: props
                        .OrderBy(p => p.SerialisedKey, StringComparer.Ordinal)
                        .ToList()));
            }
        }

        return results
            .OrderBy(c => c.Namespace, StringComparer.Ordinal)
            .ThenBy(c => c.TypeName, StringComparer.Ordinal)
            .ToList();
    }

    private static bool HasAttr(SyntaxList<AttributeListSyntax> lists, string name)
    {
        foreach (var al in lists)
            foreach (var a in al.Attributes)
                if (a.Name.ToString() == name) return true;
        return false;
    }

    private static string ExtractSerialisedKey(SyntaxList<AttributeListSyntax> attrs, string propName)
    {
        foreach (var al in attrs)
        {
            foreach (var a in al.Attributes)
            {
                var n = a.Name.ToString();
                if (n is "JsonPropertyName" or "YamlMember")
                {
                    if (a.ArgumentList is null) continue;
                    foreach (var arg in a.ArgumentList.Arguments)
                    {
                        // YamlMember(Alias = "x")
                        if (arg.NameEquals is not null && arg.NameEquals.Name.Identifier.ValueText == "Alias")
                        {
                            if (arg.Expression is LiteralExpressionSyntax l1 && l1.IsKind(SyntaxKind.StringLiteralExpression))
                                return l1.Token.ValueText;
                        }
                        // JsonPropertyName("x") — positional
                        if (arg.NameEquals is null && arg.Expression is LiteralExpressionSyntax l2 && l2.IsKind(SyntaxKind.StringLiteralExpression))
                        {
                            return l2.Token.ValueText;
                        }
                    }
                }
            }
        }
        // Convention: lowerCamelCase from PascalCase if no attribute given.
        if (string.IsNullOrEmpty(propName)) return propName;
        return char.ToLowerInvariant(propName[0]) + propName[1..];
    }

    private static string ExtractSummary(SyntaxTriviaList leading)
    {
        var trivia = leading.FirstOrDefault(t =>
            t.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia)
            || t.IsKind(SyntaxKind.MultiLineDocumentationCommentTrivia));
        if (trivia == default) return string.Empty;

        if (trivia.GetStructure() is not DocumentationCommentTriviaSyntax dox) return string.Empty;
        var summary = dox.Content.OfType<XmlElementSyntax>()
            .FirstOrDefault(e => e.StartTag.Name.LocalName.ValueText == "summary");
        if (summary is null) return string.Empty;

        var sb = new StringBuilder();
        foreach (var node in summary.Content) sb.Append(node.ToString());

        var lines = sb.ToString().Split('\n')
            .Select(l => l.TrimStart().TrimStart('/').TrimStart())
            .Where(l => l.Length > 0);
        var joined = string.Join(' ', lines).Trim();
        while (joined.Contains("  ", StringComparison.Ordinal))
            joined = joined.Replace("  ", " ");
        return joined;
    }
}
