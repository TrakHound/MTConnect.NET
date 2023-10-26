using MTConnect.SysML;
using MTConnect.SysML.CSharp;
using System.Text.Json;

var xmlPath = @"D:\TrakHound\MTConnect\MTConnectSysMLModel.xml";
//var outputPath = @"C:\temp\mtconnect-model-results";
var outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../../../src/MTConnect.NET-Common");

var mtconnectModel = MTConnectModel.Parse(xmlPath);

//var jsonOptions = new JsonSerializerOptions();
//jsonOptions.WriteIndented = true;
//jsonOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault;
//jsonOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

//var json = JsonSerializer.Serialize(mtconnectModel, options: jsonOptions);
//await File.WriteAllTextAsync(@"C:\temp\mtconnect-model.json", json);


CSharpTemplateRenderer.Render(mtconnectModel, outputPath);