using Scriban;

namespace MTConnect.SysML.Json_cppagent
{
    public static class JsonCppAgentTemplateRenderer
    {
        public static void Render(MTConnectModel mtconnectModel, string outputPath)
        {
            if (mtconnectModel != null && !string.IsNullOrEmpty(outputPath))
            {
                WriteComponents(mtconnectModel, outputPath);
                WriteEvents(mtconnectModel, outputPath);
                WriteSamples(mtconnectModel, outputPath);
            }
        }


        private static void WriteComponents(MTConnectModel mtconnectModel, string outputPath)
        {
            var componentsModel = new ComponentsModel();

            var components = mtconnectModel.DeviceInformationModel.Components.Types;
            foreach (var component in components.OrderBy(o => o.Type)) componentsModel.Types.Add(component);

            var templateFilename = $"Components.scriban";
            var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "json-cppagent", "templates", templateFilename);
            if (File.Exists(templatePath))
            {
                try
                {
                    var templateContents = File.ReadAllText(templatePath);
                    if (templateContents != null)
                    {
                        var template = Template.Parse(templateContents);
                        var result = template.Render(componentsModel);
                        if (result != null)
                        {
                            var resultPath = "Devices/JsonComponents";
                            resultPath = Path.Combine(outputPath, resultPath);
                            resultPath = $"{resultPath}.g.cs";

                            var resultDirectory = Path.GetDirectoryName(resultPath);
                            if (!Directory.Exists(resultDirectory)) Directory.CreateDirectory(resultDirectory);

                            File.WriteAllText(resultPath, result);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private static void WriteEvents(MTConnectModel mtconnectModel, string outputPath)
        {
            var dataItemsModel = new DataItemsModel();

            var dataItems = mtconnectModel.DeviceInformationModel.DataItems.Types;
            foreach (var dataItem in dataItems.Where(o => o.Category == "EVENT").OrderBy(o => o.Type)) dataItemsModel.Types.Add(dataItem);

            var templateFilename = $"Events.scriban";
            var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "json-cppagent", "templates", templateFilename);
            if (File.Exists(templatePath))
            {
                try
                {
                    var templateContents = File.ReadAllText(templatePath);
                    if (templateContents != null)
                    {
                        var template = Template.Parse(templateContents);
                        var result = template.Render(dataItemsModel);
                        if (result != null)
                        {
                            var resultPath = "Streams/JsonEvents";
                            resultPath = Path.Combine(outputPath, resultPath);
                            resultPath = $"{resultPath}.g.cs";

                            var resultDirectory = Path.GetDirectoryName(resultPath);
                            if (!Directory.Exists(resultDirectory)) Directory.CreateDirectory(resultDirectory);

                            File.WriteAllText(resultPath, result);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private static void WriteSamples(MTConnectModel mtconnectModel, string outputPath)
        {
            var dataItemsModel = new DataItemsModel();

            var dataItems = mtconnectModel.DeviceInformationModel.DataItems.Types;
            foreach (var dataItem in dataItems.Where(o => o.Category == "SAMPLE").OrderBy(o => o.Type)) dataItemsModel.Types.Add(dataItem);

            var templateFilename = $"Samples.scriban";
            var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "json-cppagent", "templates", templateFilename);
            if (File.Exists(templatePath))
            {
                try
                {
                    var templateContents = File.ReadAllText(templatePath);
                    if (templateContents != null)
                    {
                        var template = Template.Parse(templateContents);
                        var result = template.Render(dataItemsModel);
                        if (result != null)
                        {
                            var resultPath = "Streams/JsonSamples";
                            resultPath = Path.Combine(outputPath, resultPath);
                            resultPath = $"{resultPath}.g.cs";

                            var resultDirectory = Path.GetDirectoryName(resultPath);
                            if (!Directory.Exists(resultDirectory)) Directory.CreateDirectory(resultDirectory);

                            File.WriteAllText(resultPath, result);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
