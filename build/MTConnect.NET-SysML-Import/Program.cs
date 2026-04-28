using MTConnect.SysML;
using MTConnect.SysML.CSharp;
using MTConnect.SysML.Json_cppagent;
using MTConnect.SysML.Xml;
using System.Linq;
using System.Text.Json;

// SysML importer entry point. Runs on Linux / macOS / Windows / CI.
//
// Usage:
//     dotnet run --project build/MTConnect.NET-SysML-Import \
//         -- --xmi <path-to-MTConnectSysMLModel.xml> \
//            --output <repo-root> \
//            [--json-dump <path>]
//
// Flags:
//   --xmi <path>       SysML XMI file to consume. Required.
//   --output <path>    Repository root. Each subgenerator writes into its own
//                      libraries/<LibraryName>/ subtree under this root.
//                      Required.
//   --json-dump <path> Optional. Writes the parsed MTConnectModel as JSON
//                      for debugging.
//
// See build/MTConnect.NET-SysML-Import/README.md for the full usage guide,
// the "Adding a new MTConnect Standard version" runbook, and the determinism
// guarantee (regen against a pinned XMI tag must produce zero diff).

string? xmiPath = null;
string? outputRoot = null;
string? jsonDumpPath = null;

for (int i = 0; i < args.Length; i++)
{
    switch (args[i])
    {
        case "--xmi":
            xmiPath = RequireValue(args, ref i, "--xmi");
            break;
        case "--output":
            outputRoot = RequireValue(args, ref i, "--output");
            break;
        case "--json-dump":
            jsonDumpPath = RequireValue(args, ref i, "--json-dump");
            break;
        case "-h":
        case "--help":
            PrintHelp();
            return 0;
        default:
            Console.Error.WriteLine($"Unknown argument: {args[i]}");
            PrintHelp();
            return 2;
    }
}

if (string.IsNullOrEmpty(xmiPath))
{
    Console.Error.WriteLine("error: --xmi <path> is required.");
    PrintHelp();
    return 2;
}

if (!File.Exists(xmiPath))
{
    Console.Error.WriteLine($"error: XMI file not found: {xmiPath}");
    return 1;
}

if (string.IsNullOrEmpty(outputRoot))
{
    Console.Error.WriteLine("error: --output <path> is required.");
    PrintHelp();
    return 2;
}

if (!Directory.Exists(outputRoot))
{
    Console.Error.WriteLine($"error: Output root not found: {outputRoot}");
    return 1;
}

Console.WriteLine($"XMI:    {xmiPath}");
Console.WriteLine($"Output: {outputRoot}");
if (jsonDumpPath is not null)
    Console.WriteLine($"JSON:   {jsonDumpPath}");

var mtconnectModel = MTConnectModel.Parse(xmiPath);
Console.WriteLine($"Model parsed: type={mtconnectModel?.GetType().Name ?? "null"}");

if (jsonDumpPath is not null)
    RenderJsonFile(mtconnectModel, jsonDumpPath);

Console.WriteLine("Rendering C# common classes...");
RenderCommonClasses(mtconnectModel, outputRoot);
Console.WriteLine("Rendering JSON-cppagent formatters...");
RenderJsonComponents(mtconnectModel, outputRoot);
Console.WriteLine("Rendering XML formatters...");
RenderXmlComponents(mtconnectModel, outputRoot);
Console.WriteLine("Done.");
return 0;


static string RequireValue(string[] argv, ref int index, string flag)
{
    index++;
    if (index >= argv.Length)
        throw new ArgumentException($"Flag '{flag}' requires a value.");
    return argv[index];
}

static void PrintHelp()
{
    Console.WriteLine("""
        MTConnect.NET SysML Importer

        Usage:
          dotnet run --project build/MTConnect.NET-SysML-Import -- \
            --xmi <path-to-MTConnectSysMLModel.xml> \
            --output <repo-root> \
            [--json-dump <path>]

        See build/MTConnect.NET-SysML-Import/README.md for the full guide.
        """);
}

static void RenderJsonFile(MTConnectModel model, string path)
{
    var jsonOptions = new JsonSerializerOptions
    {
        WriteIndented = true,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    // --json-dump is operator-trusted (no path-traversal guard); echo the
    // resolved absolute path so the operator can verify exactly where the
    // dump landed when running with a relative path or a sibling-clone
    // launchSettings profile.
    var resolved = Path.GetFullPath(path);
    Console.WriteLine($"JSON dump: writing to {resolved}");

    var dir = Path.GetDirectoryName(resolved);
    if (!string.IsNullOrEmpty(dir))
        Directory.CreateDirectory(dir);

    var json = JsonSerializer.Serialize(model, options: jsonOptions);
    File.WriteAllText(resolved, json);
}

static void RenderCommonClasses(MTConnectModel model, string outputRoot)
{
    var outputPath = Path.Combine(outputRoot, "libraries", "MTConnect.NET-Common");
    if (!Directory.Exists(outputPath))
        throw new DirectoryNotFoundException($"MTConnect.NET-Common not found under output root: {outputPath}");

    CSharpTemplateRenderer.Render(model, outputPath);
}

static void RenderJsonComponents(MTConnectModel model, string outputRoot)
{
    var outputPath = Path.Combine(outputRoot, "libraries", "MTConnect.NET-JSON-cppagent");
    if (!Directory.Exists(outputPath))
        throw new DirectoryNotFoundException($"MTConnect.NET-JSON-cppagent not found under output root: {outputPath}");

    JsonCppAgentTemplateRenderer.Render(model, outputPath);
}

static void RenderXmlComponents(MTConnectModel model, string outputRoot)
{
    var outputPath = Path.Combine(outputRoot, "libraries", "MTConnect.NET-XML");
    if (!Directory.Exists(outputPath))
        throw new DirectoryNotFoundException($"MTConnect.NET-XML not found under output root: {outputPath}");

    XmlTemplateRenderer.Render(model, outputPath);
}
