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

            RenderTo("Components.scriban", componentsModel, "Devices/JsonComponents", outputPath);
        }

        private static void WriteEvents(MTConnectModel mtconnectModel, string outputPath)
        {
            var dataItemsModel = new DataItemsModel();

            var dataItems = mtconnectModel.DeviceInformationModel.DataItems.Types;
            foreach (var dataItem in dataItems.Where(o => o.Category == "EVENT").OrderBy(o => o.Type)) dataItemsModel.Types.Add(dataItem);

            RenderTo("Events.scriban", dataItemsModel, "Streams/JsonEvents", outputPath);
        }

        private static void WriteSamples(MTConnectModel mtconnectModel, string outputPath)
        {
            var dataItemsModel = new DataItemsModel();

            var dataItems = mtconnectModel.DeviceInformationModel.DataItems.Types;
            foreach (var dataItem in dataItems.Where(o => o.Category == "SAMPLE").OrderBy(o => o.Type)) dataItemsModel.Types.Add(dataItem);

            RenderTo("Samples.scriban", dataItemsModel, "Streams/JsonSamples", outputPath);
        }

        private static void WriteCuttingToolMeasurements(MTConnectModel mtconnectModel, string outputPath)
        {
            var measurementsModel = new CuttingToolMeasurementsModel();

            var measurements = mtconnectModel.AssetInformationModel.CuttingTools.Classes.Where(o => typeof(MTConnectMeasurementModel).IsAssignableFrom(o.GetType()));
            foreach (var measurement in measurements.OrderBy(o => o.Name)) measurementsModel.Types.Add((MTConnectMeasurementModel)measurement);

            RenderTo("Measurements.scriban", measurementsModel, "Assets/CuttingTools/JsonMeasurements", outputPath);
        }

        // Loads the named Scriban template from Json-cppagent/Templates,
        // renders against the supplied model, and writes the result to
        // <outputPath>/<outputRelative>.g.cs (creating intermediate
        // directories as needed). Centralises the load -> render -> write
        // sequence the four Write* methods would otherwise repeat verbatim.
        private static void RenderTo(string templateName, object model, string outputRelative, string outputPath)
        {
            var template = TemplateLoader.LoadOrThrow("Json-cppagent", "Templates", templateName);
            var result = template.Render(model);
            if (result == null) return;

            var resultPath = Path.Combine(outputPath, outputRelative) + ".g.cs";
            // Path.GetDirectoryName may return null/empty when outputPath is
            // a bare relative path (`--output .`); fall back to current
            // directory so EnsureDirectory does not throw on null.
            var resultDirectory = Path.GetDirectoryName(resultPath);
            if (string.IsNullOrEmpty(resultDirectory)) resultDirectory = ".";
            TemplateLoader.EnsureDirectory(resultDirectory);
            File.WriteAllText(resultPath, result);
        }
    }
}
