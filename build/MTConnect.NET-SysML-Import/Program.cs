using MTConnect.SysML;
using MTConnect.SysML.CSharp;
using MTConnect.SysML.Json_cppagent;
using MTConnect.SysML.Xml;
using System.Text.Json;

//var xmlPath = @"D:\TrakHound\MTConnect\MTConnectSysMLModel.xml";
//var xmlPath = @"D:\TrakHound\MTConnect\Standard\v2.4\MTConnectSysMLModel.xml";
var xmlPath = @"D:\TrakHound\MTConnect\Standard\v2.5\MTConnectSysMLModel.xml";

var mtconnectModel = MTConnectModel.Parse(xmlPath);

RenderJsonFile();
RenderCommonClasses();
RenderJsonComponents();
RenderXmlComponents();


void RenderJsonFile()
{
    var jsonOptions = new JsonSerializerOptions();
    jsonOptions.WriteIndented = true;
    jsonOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault;
    jsonOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

    var json = JsonSerializer.Serialize(mtconnectModel, options: jsonOptions);
    File.WriteAllText(@"C:\temp\mtconnect-model.json", json);
}

void RenderCommonClasses()
{
    //var outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"C:\temp\mtconnect-sysml-build");
    var outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../../../libraries/MTConnect.NET-Common");

    //// Clear Generated Files
    //var files = Directory.GetFiles(outputPath, "*.g.cs", SearchOption.AllDirectories);
    //foreach (var file in files) File.Delete(file);

    CSharpTemplateRenderer.Render(mtconnectModel, outputPath);
}

void RenderJsonComponents()
{
    var outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../../../libraries/MTConnect.NET-JSON-cppagent");

    JsonCppAgentTemplateRenderer.Render(mtconnectModel, outputPath);
}

void RenderXmlComponents()
{
    var outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../../../libraries/MTConnect.NET-XML");

    XmlTemplateRenderer.Render(mtconnectModel, outputPath);
}