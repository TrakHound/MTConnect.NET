using MTConnect.SysML;
using MTConnect.SysML.CSharp;
using MTConnect.SysML.Json_cppagent;
using System.Text.Json;

var xmlPath = @"D:\TrakHound\MTConnect\MTConnectSysMLModel.xml";
//var outputPath = @"C:\temp\mtconnect-model-results";


var mtconnectModel = MTConnectModel.Parse(xmlPath);

//var jsonOptions = new JsonSerializerOptions();
//jsonOptions.WriteIndented = true;
//jsonOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault;
//jsonOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

//var json = JsonSerializer.Serialize(mtconnectModel, options: jsonOptions);
//await File.WriteAllTextAsync(@"C:\temp\mtconnect-model.json", json);

RenderCommonClasses();
RenderJsonComponents();



void RenderCommonClasses()
{
    var outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../../../src/MTConnect.NET-Common");

    CSharpTemplateRenderer.Render(mtconnectModel, outputPath);
}

void RenderJsonComponents()
{
    var outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../../../src/MTConnect.NET-JSON-cppagent");

    JsonCppAgentTemplateRenderer.Render(mtconnectModel, outputPath);
}