using MTConnect.SysML.Models.Assets;

namespace MTConnect.SysML.Xml
{
    public static class XmlTemplateRenderer
    {
        public static void Render(MTConnectModel mtconnectModel, string outputPath)
        {
            if (mtconnectModel != null && !string.IsNullOrEmpty(outputPath))
            {
                // All three Xml templates render the same CuttingToolMeasurementsModel —
                // build it once, then drive the three (template, output) pairs through
                // a shared helper. Output is byte-identical to the previous three-method
                // form; the templates differ only in which model fields they read.
                var measurementsModel = BuildCuttingToolMeasurementsModel(mtconnectModel);
                var renders = new (string Template, string OutputRelative)[]
                {
                    ("XmlMeasurements.scriban",      "Assets/CuttingTools/XmlMeasurements"),
                    ("XmlCuttingToolLifeCycle.scriban", "Assets/CuttingTools/XmlCuttingToolLifeCycle"),
                    ("XmlCuttingItem.scriban",       "Assets/CuttingTools/XmlCuttingItem"),
                };
                foreach (var (template, output) in renders)
                {
                    RenderTo(template, measurementsModel, output, outputPath);
                }
            }
        }


        private static CuttingToolMeasurementsModel BuildCuttingToolMeasurementsModel(MTConnectModel mtconnectModel)
        {
            var model = new CuttingToolMeasurementsModel();

            var measurements = mtconnectModel.AssetInformationModel.CuttingTools.Classes.Where(o => typeof(MTConnectMeasurementModel).IsAssignableFrom(o.GetType()));
            foreach (var measurement in measurements.OrderBy(o => o.Name)) model.Types.Add((MTConnectMeasurementModel)measurement);

            return model;
        }

        // Loads the named Scriban template from Xml/Templates, renders against
        // the supplied model, and writes the result to <outputPath>/<outputRelative>.g.cs
        // (creating intermediate directories as needed).
        private static void RenderTo(string templateName, object model, string outputRelative, string outputPath)
        {
            var template = TemplateLoader.LoadOrThrow("Xml", "Templates", templateName);
            var result = template.Render(model);
            if (result == null) return;

            var resultPath = Path.Combine(outputPath, outputRelative) + ".g.cs";
            var resultDirectory = Path.GetDirectoryName(resultPath);
            TemplateLoader.EnsureDirectory(resultDirectory);
            File.WriteAllText(resultPath, result);
        }
    }
}
