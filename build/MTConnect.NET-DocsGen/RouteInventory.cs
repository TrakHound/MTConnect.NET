// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;

namespace MTConnect.NET_DocsGen;

/// <summary>
/// Describes a single discovered HTTP endpoint. The inventory is
/// produced by Roslyn syntax-only inspection; no semantic model or
/// compiled assembly is required.
/// </summary>
public sealed record EndpointInfo(
    string Source,              // "Ceen" | "AspNetCore"
    string Surface,             // "Agent" | "Admin"
    string Method,              // GET / POST / PUT / DELETE / ANY
    string PathTemplate,        // e.g. "/probe", "/{deviceKey}/sample"
    string Handler,             // handler class or controller.action
    string Summary,             // XML-doc summary (may be empty)
    IReadOnlyList<EndpointParam> Parameters,
    IReadOnlyList<string> ResponseStatusCodes,
    string FileRelativePath,
    int Line);

/// <summary>
/// A single query / route / body parameter on an endpoint.
/// </summary>
public sealed record EndpointParam(
    string Name,
    string Kind,                // Route / Query / Body / Header
    string Type,
    string? DefaultValue,
    string Summary);

/// <summary>
/// Builds the endpoint inventory by walking every .cs file in the
/// `libraries/MTConnect.NET-HTTP` and `libraries/MTConnect.NET-HTTP-AspNetCore`
/// trees. Two collectors run side by side and emit a single
/// deterministic ordered list:
///
///   1. Ceen routes — `serverConfig.AddRoute("/path", handler)`
///   2. AspNetCore controllers — `[HttpGet("/path")]` etc.
/// </summary>
public static class RouteInventory
{
    public static IReadOnlyList<EndpointInfo> Collect(string repoRoot)
    {
        var results = new List<EndpointInfo>();

        var ceenRoot = Path.Combine(repoRoot, "libraries", "MTConnect.NET-HTTP");
        var aspNetRoot = Path.Combine(repoRoot, "libraries", "MTConnect.NET-HTTP-AspNetCore");

        if (Directory.Exists(ceenRoot))
        {
            CollectCeen(ceenRoot, repoRoot, results);
        }
        if (Directory.Exists(aspNetRoot))
        {
            CollectAspNetCore(aspNetRoot, repoRoot, results);
        }

        // Deterministic sort: source, then method, then path.
        return results
            .OrderBy(e => e.Source, StringComparer.Ordinal)
            .ThenBy(e => e.Method, StringComparer.Ordinal)
            .ThenBy(e => e.PathTemplate, StringComparer.Ordinal)
            .ThenBy(e => e.Handler, StringComparer.Ordinal)
            .ToList();
    }

    // ----- Ceen ---------------------------------------------------------

    private static void CollectCeen(string root, string repoRoot, List<EndpointInfo> sink)
    {
        var serverFile = Path.Combine(root, "Servers", "MTConnectHttpServer.cs");
        if (!File.Exists(serverFile)) return;

        var text = File.ReadAllText(serverFile);
        var tree = CSharpSyntaxTree.ParseText(text, path: serverFile);
        var rel = Path.GetRelativePath(repoRoot, serverFile).Replace('\\', '/');

        foreach (var inv in tree.GetRoot()
                     .DescendantNodes()
                     .OfType<InvocationExpressionSyntax>())
        {
            if (inv.Expression is not MemberAccessExpressionSyntax member) continue;
            if (member.Name.Identifier.ValueText != "AddRoute") continue;

            var args = inv.ArgumentList.Arguments;
            // Only the `AddRoute(string, handler)` overload yields a
            // user-visible path template. The handler-only overloads
            // are dispatchers (e.g. PUT/POST/device-root) that gate by
            // method/host inside the closure — we surface those by
            // inspecting the closure body separately below.
            if (args.Count == 2 &&
                args[0].Expression is LiteralExpressionSyntax lit &&
                lit.IsKind(SyntaxKind.StringLiteralExpression))
            {
                var path = lit.Token.ValueText;
                var handler = ExtractIdentifier(args[1].Expression);
                var (summary, method, paramList, statuses) = LookupHandlerMetadata(handler, root);
                sink.Add(new EndpointInfo(
                    Source: "Ceen",
                    Surface: "Agent",
                    Method: method,
                    PathTemplate: NormalisePath(path),
                    Handler: handler,
                    Summary: summary,
                    Parameters: paramList,
                    ResponseStatusCodes: statuses,
                    FileRelativePath: rel,
                    Line: lit.GetLocation().GetLineSpan().StartLinePosition.Line + 1));
            }
            else if (args.Count == 1 &&
                     args[0].Expression is ParenthesizedLambdaExpressionSyntax lambda)
            {
                // Method-gating dispatcher closures: surface as ANY of
                // the gated method on "*" so the table still shows them.
                var (gateMethod, targetHandler) = SniffLambdaGate(lambda);
                if (gateMethod is not null)
                {
                    sink.Add(new EndpointInfo(
                        Source: "Ceen",
                        Surface: "Agent",
                        Method: gateMethod,
                        PathTemplate: "(any)",
                        Handler: targetHandler ?? "(dispatcher)",
                        Summary: "Method-gated dispatcher; routes the request to the handler matching the request method.",
                        Parameters: Array.Empty<EndpointParam>(),
                        ResponseStatusCodes: Array.Empty<string>(),
                        FileRelativePath: rel,
                        Line: lambda.GetLocation().GetLineSpan().StartLinePosition.Line + 1));
                }
            }
        }
    }

    private static (string? method, string? handler) SniffLambdaGate(ParenthesizedLambdaExpressionSyntax lambda)
    {
        // Look for an invocation like `await DeviceRootHandler(probeHandler, context)`
        // and a comparison `httpRequest.Method == HttpMethod.Put.Method` in the
        // referenced helper. We can't easily resolve cross-method without a
        // semantic model, so just check the lambda body text.
        var bodyText = lambda.Body.ToString();
        string? method = null;
        if (bodyText.Contains("PutHandler", StringComparison.Ordinal)) method = "PUT";
        else if (bodyText.Contains("PostHandler", StringComparison.Ordinal)) method = "POST";
        else if (bodyText.Contains("DeviceRootHandler", StringComparison.Ordinal)) method = "GET";

        string? handler = null;
        foreach (var word in new[] { "putHandler", "postHandler", "probeHandler", "currentHandler", "sampleHandler", "assetsHandler", "assetHandler" })
        {
            if (bodyText.Contains(word, StringComparison.Ordinal)) { handler = word; break; }
        }

        return (method, handler);
    }

    private static string ExtractIdentifier(ExpressionSyntax expr) => expr switch
    {
        IdentifierNameSyntax id => id.Identifier.ValueText,
        MemberAccessExpressionSyntax m => m.Name.Identifier.ValueText,
        _ => expr.ToString()
    };

    // Map from Ceen handler-variable identifier (as it appears at the
    // AddRoute call site) to the concrete handler class name. The class
    // names are structural and stable; the route table lives in source
    // and would be tedious to re-derive via lexical-scope analysis
    // without a semantic model. The summary text, by contrast, is read
    // straight out of each handler class's /// doc-comment.
    private static readonly IReadOnlyDictionary<string, string> CeenHandlerTypeNames = new Dictionary<string, string>
    {
        ["probeHandler"] = "MTConnectProbeResponseHandler",
        ["currentHandler"] = "MTConnectCurrentResponseHandler",
        ["sampleHandler"] = "MTConnectSampleResponseHandler",
        ["assetsHandler"] = "MTConnectAssetsResponseHandler",
        ["assetHandler"] = "MTConnectAssetResponseHandler",
        ["staticHandler"] = "MTConnectStaticResponseHandler",
        ["putHandler"] = "MTConnectPutResponseHandler",
        ["postHandler"] = "MTConnectPostResponseHandler",
    };

    // HTTP method per handler class. Ceen routes the method-gated
    // dispatcher closures into PUT/POST handlers regardless of the
    // AddRoute path, so the method is a property of the handler class
    // itself (not the AddRoute call).
    private static readonly IReadOnlyDictionary<string, string> CeenHandlerMethods = new Dictionary<string, string>
    {
        ["MTConnectProbeResponseHandler"] = "GET",
        ["MTConnectCurrentResponseHandler"] = "GET",
        ["MTConnectSampleResponseHandler"] = "GET",
        ["MTConnectAssetsResponseHandler"] = "GET",
        ["MTConnectAssetResponseHandler"] = "GET",
        ["MTConnectStaticResponseHandler"] = "GET",
        ["MTConnectPutResponseHandler"] = "PUT",
        ["MTConnectPostResponseHandler"] = "POST",
    };

    // Parameter shapes per handler class. Ceen parses query parameters
    // imperatively inside OnRequestReceived, so the surface is encoded
    // structurally here to give the reference page meaningful columns;
    // the *summary text* still flows from /// on the handler class.
    private static readonly IReadOnlyDictionary<string, IReadOnlyList<EndpointParam>> CeenHandlerParameters
        = new Dictionary<string, IReadOnlyList<EndpointParam>>
    {
        ["MTConnectProbeResponseHandler"] = new EndpointParam[]
        {
            new("deviceType", "Query", "string", null, "Optional device-type filter."),
            new("version", "Query", "string", null, "Target MTConnect Standard version of the response document."),
            new("documentFormat", "Query", "string", "xml", "Response document format (xml | json | json-cppagent)."),
            new("validationLevel", "Query", "int", null, "0=ignore, 1=warning, 2=remove, 3=strict."),
            new("indentOutput", "Query", "bool", null, "Pretty-print the response document."),
            new("outputComments", "Query", "bool", null, "Emit comments / annotations in the response document."),
        },
        ["MTConnectCurrentResponseHandler"] = new EndpointParam[]
        {
            new("path", "Query", "string", null, "XPath that filters the data items included in the response."),
            new("at", "Query", "ulong", null, "Sequence number anchoring the snapshot."),
            new("interval", "Query", "int", null, "Streaming interval in milliseconds; switches to multipart streaming when > 0."),
            new("heartbeat", "Query", "int", "10000", "Heartbeat interval for the streaming response."),
            new("deviceType", "Query", "string", null, "Optional device-type filter."),
            new("version", "Query", "string", null, "Target MTConnect Standard version of the response document."),
            new("documentFormat", "Query", "string", "xml", "Response document format."),
            new("indentOutput", "Query", "bool", null, "Pretty-print the response document."),
            new("outputComments", "Query", "bool", null, "Emit comments / annotations in the response document."),
        },
        ["MTConnectSampleResponseHandler"] = new EndpointParam[]
        {
            new("path", "Query", "string", null, "XPath that filters the data items included in the response."),
            new("from", "Query", "ulong", null, "Sequence number lower bound."),
            new("to", "Query", "ulong", null, "Sequence number upper bound."),
            new("count", "Query", "int", "100", "Maximum number of observations."),
            new("interval", "Query", "int", null, "Streaming interval in milliseconds; switches to multipart streaming when > 0."),
            new("heartbeat", "Query", "int", "10000", "Heartbeat interval for the streaming response."),
            new("deviceType", "Query", "string", null, "Optional device-type filter."),
            new("version", "Query", "string", null, "Target MTConnect Standard version of the response document."),
            new("documentFormat", "Query", "string", "xml", "Response document format."),
            new("indentOutput", "Query", "bool", null, "Pretty-print the response document."),
            new("outputComments", "Query", "bool", null, "Emit comments / annotations in the response document."),
        },
        ["MTConnectAssetsResponseHandler"] = new EndpointParam[]
        {
            new("type", "Query", "string", null, "Asset type filter (e.g. CuttingTool)."),
            new("removed", "Query", "bool", null, "Include removed assets when true."),
            new("count", "Query", "int", null, "Maximum number of assets."),
            new("documentFormat", "Query", "string", "xml", "Response document format."),
            new("indentOutput", "Query", "bool", null, "Pretty-print the response document."),
        },
        ["MTConnectAssetResponseHandler"] = new EndpointParam[]
        {
            new("assetId", "Route", "string", null, "Asset identifier captured from the trailing path segment."),
            new("documentFormat", "Query", "string", "xml", "Response document format."),
        },
        ["MTConnectPutResponseHandler"] = new EndpointParam[]
        {
            new("(form / query)", "Body", "Dictionary<string,string>", null, "DataItemId=Value entries to enqueue as observations."),
        },
        ["MTConnectPostResponseHandler"] = new EndpointParam[]
        {
            new("(body)", "Body", "string", null, "Asset document payload."),
        },
    };

    // Cache of parsed /// summary text per handler-class file path, so
    // we only Roslyn-parse each file once even though several variables
    // (probeHandler, the AddRoute("/", probeHandler) alias, etc.) all
    // resolve back to the same class.
    private static readonly Dictionary<string, string> _ceenSummaryCache = new(StringComparer.Ordinal);

    /// <summary>
    /// Builds the per-endpoint metadata triple for a single Ceen
    /// AddRoute call. The HTTP method and the parameter shape are
    /// structural properties of the handler class and are encoded
    /// statically. The free-text summary, however, is parsed straight
    /// out of the handler class's `///` doc-comment via Roslyn — so the
    /// docs site stays in lock-step with whatever the handler classes
    /// say about themselves.
    /// </summary>
    private static (string Summary, string Method, IReadOnlyList<EndpointParam> Params, IReadOnlyList<string> Statuses)
        LookupHandlerMetadata(string handlerVarName, string ceenRoot)
    {
        if (!CeenHandlerTypeNames.TryGetValue(handlerVarName, out var typeName))
        {
            typeName = handlerVarName;
        }

        var method = CeenHandlerMethods.TryGetValue(typeName, out var m) ? m : "ANY";

        var summary = ReadCeenHandlerSummary(typeName, ceenRoot);

        var parameters = CeenHandlerParameters.TryGetValue(typeName, out var ps)
            ? ps
            : Array.Empty<EndpointParam>();

        IReadOnlyList<string> statuses = method == "ANY"
            ? Array.Empty<string>()
            : new[] { "200", "400", "404", "406", "500" };

        return (summary, method, parameters, statuses);
    }

    private static string ReadCeenHandlerSummary(string typeName, string ceenRoot)
    {
        var path = Path.Combine(ceenRoot, "Servers", typeName + ".cs");
        if (_ceenSummaryCache.TryGetValue(path, out var cached)) return cached;

        if (!File.Exists(path))
        {
            _ceenSummaryCache[path] = string.Empty;
            return string.Empty;
        }

        var text = File.ReadAllText(path);
        var tree = CSharpSyntaxTree.ParseText(text, path: path);
        var cls = tree.GetRoot()
            .DescendantNodes()
            .OfType<ClassDeclarationSyntax>()
            .FirstOrDefault(c => c.Identifier.ValueText == typeName);

        if (cls is null)
        {
            _ceenSummaryCache[path] = string.Empty;
            return string.Empty;
        }

        var trivia = cls.GetLeadingTrivia()
            .FirstOrDefault(t => t.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia)
                              || t.IsKind(SyntaxKind.MultiLineDocumentationCommentTrivia));
        if (trivia == default || trivia.GetStructure() is not DocumentationCommentTriviaSyntax dox)
        {
            _ceenSummaryCache[path] = string.Empty;
            return string.Empty;
        }

        var summary = dox.Content.OfType<XmlElementSyntax>()
            .FirstOrDefault(e => e.StartTag.Name.LocalName.ValueText == "summary");
        if (summary is null)
        {
            _ceenSummaryCache[path] = string.Empty;
            return string.Empty;
        }

        var sb = new StringBuilder();
        foreach (var node in summary.Content) sb.Append(node.ToString());
        var result = CleanXmlText(sb.ToString());

        _ceenSummaryCache[path] = result;
        return result;
    }

    private static string NormalisePath(string p) =>
        p.Length == 0 ? "/" : p;

    // ----- AspNetCore ---------------------------------------------------

    private static void CollectAspNetCore(string root, string repoRoot, List<EndpointInfo> sink)
    {
        var controllersDir = Path.Combine(root, "Http", "Controllers");
        if (!Directory.Exists(controllersDir)) return;

        foreach (var file in Directory.GetFiles(controllersDir, "*.cs"))
        {
            var text = File.ReadAllText(file);
            var rel = Path.GetRelativePath(repoRoot, file).Replace('\\', '/');
            var tree = CSharpSyntaxTree.ParseText(text, path: file);

            foreach (var cls in tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>())
            {
                if (!IsApiController(cls)) continue;

                var basePath = ExtractRoute(cls.AttributeLists);

                foreach (var method in cls.Members.OfType<MethodDeclarationSyntax>())
                {
                    var httpAttrs = method.AttributeLists
                        .SelectMany(al => al.Attributes)
                        .Where(a => IsHttpAttr(a, out _))
                        .ToList();
                    if (httpAttrs.Count == 0) continue;

                    var summary = ExtractXmlSummary(method);
                    var parameters = ExtractParameters(method);
                    var statuses = ExtractResponseStatuses(method);

                    foreach (var attr in httpAttrs)
                    {
                        IsHttpAttr(attr, out var verb);
                        var sub = ExtractFirstStringArg(attr);
                        var full = Combine(basePath, sub);

                        sink.Add(new EndpointInfo(
                            Source: "AspNetCore",
                            Surface: "Agent",
                            Method: verb!,
                            PathTemplate: full,
                            Handler: $"{cls.Identifier.ValueText}.{method.Identifier.ValueText}",
                            Summary: summary,
                            Parameters: parameters,
                            ResponseStatusCodes: statuses,
                            FileRelativePath: rel,
                            Line: attr.GetLocation().GetLineSpan().StartLinePosition.Line + 1));
                    }
                }
            }
        }
    }

    private static bool IsApiController(ClassDeclarationSyntax cls)
    {
        foreach (var al in cls.AttributeLists)
        {
            foreach (var a in al.Attributes)
            {
                if (a.Name.ToString() == "ApiController") return true;
            }
        }
        return false;
    }

    private static string ExtractRoute(SyntaxList<AttributeListSyntax> attrs)
    {
        foreach (var al in attrs)
        {
            foreach (var a in al.Attributes)
            {
                if (a.Name.ToString() != "Route") continue;
                return ExtractFirstStringArg(a) ?? string.Empty;
            }
        }
        return string.Empty;
    }

    private static bool IsHttpAttr(AttributeSyntax a, out string? verb)
    {
        var name = a.Name.ToString();
        verb = name switch
        {
            "HttpGet" => "GET",
            "HttpPost" => "POST",
            "HttpPut" => "PUT",
            "HttpDelete" => "DELETE",
            "HttpPatch" => "PATCH",
            _ => null
        };
        return verb is not null;
    }

    private static string? ExtractFirstStringArg(AttributeSyntax a)
    {
        if (a.ArgumentList is null) return null;
        var first = a.ArgumentList.Arguments.FirstOrDefault();
        if (first?.Expression is LiteralExpressionSyntax lit &&
            lit.IsKind(SyntaxKind.StringLiteralExpression))
        {
            return lit.Token.ValueText;
        }
        return null;
    }

    private static string Combine(string basePath, string? sub)
    {
        var b = basePath.TrimEnd('/');
        if (string.IsNullOrEmpty(sub)) return string.IsNullOrEmpty(b) ? "/" : b;
        var s = sub.TrimStart('/');
        if (string.IsNullOrEmpty(b)) return "/" + s;
        return b + "/" + s;
    }

    private static string ExtractXmlSummary(MethodDeclarationSyntax method)
    {
        var trivia = method.GetLeadingTrivia()
            .FirstOrDefault(t => t.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia)
                              || t.IsKind(SyntaxKind.MultiLineDocumentationCommentTrivia));
        if (trivia == default) return string.Empty;

        var structure = trivia.GetStructure() as DocumentationCommentTriviaSyntax;
        if (structure is null) return string.Empty;

        var summary = structure.Content.OfType<XmlElementSyntax>()
            .FirstOrDefault(e => e.StartTag.Name.LocalName.ValueText == "summary");
        if (summary is null) return string.Empty;

        var sb = new StringBuilder();
        foreach (var node in summary.Content)
        {
            sb.Append(node.ToString());
        }
        return CleanXmlText(sb.ToString());
    }

    private static IReadOnlyList<EndpointParam> ExtractParameters(MethodDeclarationSyntax method)
    {
        // Parameter summaries are in the <param name="x">..</param> nodes
        // sitting alongside <summary>.
        var paramSummaries = new Dictionary<string, string>(StringComparer.Ordinal);
        var trivia = method.GetLeadingTrivia()
            .FirstOrDefault(t => t.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia)
                              || t.IsKind(SyntaxKind.MultiLineDocumentationCommentTrivia));
        if (trivia != default && trivia.GetStructure() is DocumentationCommentTriviaSyntax dox)
        {
            foreach (var el in dox.Content.OfType<XmlElementSyntax>())
            {
                if (el.StartTag.Name.LocalName.ValueText != "param") continue;
                var nameAttr = el.StartTag.Attributes.OfType<XmlNameAttributeSyntax>().FirstOrDefault();
                if (nameAttr is null) continue;
                var name = nameAttr.Identifier.Identifier.ValueText;
                var sb = new StringBuilder();
                foreach (var node in el.Content) sb.Append(node.ToString());
                paramSummaries[name] = CleanXmlText(sb.ToString());
            }
        }

        var results = new List<EndpointParam>();
        if (method.ParameterList is null) return results;

        foreach (var p in method.ParameterList.Parameters)
        {
            string kind = "Query";
            foreach (var al in p.AttributeLists)
            {
                foreach (var a in al.Attributes)
                {
                    var n = a.Name.ToString();
                    if (n == "FromRoute") kind = "Route";
                    else if (n == "FromQuery") kind = "Query";
                    else if (n == "FromBody") kind = "Body";
                    else if (n == "FromHeader") kind = "Header";
                }
            }
            var def = p.Default?.Value.ToString();
            paramSummaries.TryGetValue(p.Identifier.ValueText, out var summary);
            results.Add(new EndpointParam(
                Name: p.Identifier.ValueText,
                Kind: kind,
                Type: p.Type?.ToString() ?? "object",
                DefaultValue: def,
                Summary: summary ?? string.Empty));
        }
        return results;
    }

    private static IReadOnlyList<string> ExtractResponseStatuses(MethodDeclarationSyntax method)
    {
        var results = new List<string>();
        foreach (var al in method.AttributeLists)
        {
            foreach (var a in al.Attributes)
            {
                if (a.Name.ToString() != "ProducesResponseType") continue;
                var arg = a.ArgumentList?.Arguments.FirstOrDefault()?.Expression?.ToString();
                if (string.IsNullOrEmpty(arg)) continue;
                // "StatusCodes.Status200OK" -> "200"
                var lastDot = arg.LastIndexOf('.');
                var token = lastDot >= 0 ? arg[(lastDot + 1)..] : arg;
                var digits = new string(token.Where(char.IsDigit).ToArray());
                if (digits.Length > 0) results.Add(digits);
            }
        }
        return results.Distinct().OrderBy(s => s, StringComparer.Ordinal).ToList();
    }

    private static string CleanXmlText(string raw)
    {
        var lines = raw.Split('\n')
            .Select(l => l.TrimStart().TrimStart('/').TrimStart())
            .Where(l => l.Length > 0);
        var joined = string.Join(' ', lines).Trim();
        // Collapse double spaces.
        while (joined.Contains("  ", StringComparison.Ordinal))
            joined = joined.Replace("  ", " ");
        return joined;
    }
}
