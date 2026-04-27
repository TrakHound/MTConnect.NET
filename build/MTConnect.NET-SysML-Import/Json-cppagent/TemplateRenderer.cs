using MTConnect.SysML.Models.Assets;

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
                WriteCuttingToolMeasurements(mtconnectModel, outputPath);
            }
        }


        private static void WriteComponents(MTConnectModel mtconnectModel, string outputPath)
        {
            var componentsModel = new ComponentsModel();

            var components = mtconnectModel.DeviceInformationModel.Components.Types;
            foreach (var component in components.OrderBy(o => o.Type)) componentsModel.Types.Add(component);

            var template = TemplateLoader.LoadOrThrow("Json-cppagent", "Templates", "Components.scriban");
            var result = template.Render(componentsModel);
            if (result == null) return;

            var resultPath = Path.Combine(outputPath, "Devices/JsonComponents") + ".g.cs";
            var resultDirectory = Path.GetDirectoryName(resultPath);
            TemplateLoader.EnsureDirectory(resultDirectory);
            File.WriteAllText(resultPath, result);
        }

        private static void WriteEvents(MTConnectModel mtconnectModel, string outputPath)
        {
            var dataItemsModel = new DataItemsModel();

            var dataItems = mtconnectModel.DeviceInformationModel.DataItems.Types;
            foreach (var dataItem in dataItems.Where(o => o.Category == "EVENT").OrderBy(o => o.Type)) dataItemsModel.Types.Add(dataItem);

            var template = TemplateLoader.LoadOrThrow("Json-cppagent", "Templates", "Events.scriban");
            var result = template.Render(dataItemsModel);
            if (result == null) return;

            var resultPath = Path.Combine(outputPath, "Streams/JsonEvents") + ".g.cs";
            var resultDirectory = Path.GetDirectoryName(resultPath);
            TemplateLoader.EnsureDirectory(resultDirectory);
            File.WriteAllText(resultPath, result);
        }

        private static void WriteSamples(MTConnectModel mtconnectModel, string outputPath)
        {
            var dataItemsModel = new DataItemsModel();

            var dataItems = mtconnectModel.DeviceInformationModel.DataItems.Types;
            foreach (var dataItem in dataItems.Where(o => o.Category == "SAMPLE").OrderBy(o => o.Type)) dataItemsModel.Types.Add(dataItem);

            var template = TemplateLoader.LoadOrThrow("Json-cppagent", "Templates", "Samples.scriban");
            var result = template.Render(dataItemsModel);
            if (result == null) return;

            var resultPath = Path.Combine(outputPath, "Streams/JsonSamples") + ".g.cs";
            var resultDirectory = Path.GetDirectoryName(resultPath);
            TemplateLoader.EnsureDirectory(resultDirectory);
            File.WriteAllText(resultPath, result);
        }

        private static void WriteCuttingToolMeasurements(MTConnectModel mtconnectModel, string outputPath)
        {
            var measurementsModel = new CuttingToolMeasurementsModel();

            var measurements = mtconnectModel.AssetInformationModel.CuttingTools.Classes.Where(o => typeof(MTConnectMeasurementModel).IsAssignableFrom(o.GetType()));
            foreach (var measurement in measurements.OrderBy(o => o.Name)) measurementsModel.Types.Add((MTConnectMeasurementModel)measurement);

            var template = TemplateLoader.LoadOrThrow("Json-cppagent", "Templates", "Measurements.scriban");
            var result = template.Render(measurementsModel);
            if (result == null) return;

            var resultPath = Path.Combine(outputPath, "Assets/CuttingTools/JsonMeasurements") + ".g.cs";
            var resultDirectory = Path.GetDirectoryName(resultPath);
            TemplateLoader.EnsureDirectory(resultDirectory);
            File.WriteAllText(resultPath, result);
        }
    }
}
