using MTConnect.SysML.Models.Assets;

namespace MTConnect.SysML.Xml
{
    public static class XmlTemplateRenderer
    {
        public static void Render(MTConnectModel mtconnectModel, string outputPath)
        {
            if (mtconnectModel != null && !string.IsNullOrEmpty(outputPath))
            {
                WriteCuttingToolMeasurements(mtconnectModel, outputPath);
                WriteCuttingToolLifeCycle(mtconnectModel, outputPath);
                WriteCuttingItem(mtconnectModel, outputPath);
            }
        }


        private static void WriteCuttingToolMeasurements(MTConnectModel mtconnectModel, string outputPath)
        {
            var measurementsModel = new CuttingToolMeasurementsModel();

            var measurements = mtconnectModel.AssetInformationModel.CuttingTools.Classes.Where(o => typeof(MTConnectMeasurementModel).IsAssignableFrom(o.GetType()));
            foreach (var measurement in measurements.OrderBy(o => o.Name)) measurementsModel.Types.Add((MTConnectMeasurementModel)measurement);

            var template = TemplateLoader.LoadOrThrow("Xml", "Templates", "XmlMeasurements.scriban");
            var result = template.Render(measurementsModel);
            if (result == null) return;

            var resultPath = Path.Combine(outputPath, "Assets/CuttingTools/XmlMeasurements") + ".g.cs";
            var resultDirectory = Path.GetDirectoryName(resultPath);
            TemplateLoader.EnsureDirectory(resultDirectory);
            File.WriteAllText(resultPath, result);
        }

        private static void WriteCuttingToolLifeCycle(MTConnectModel mtconnectModel, string outputPath)
        {
            var measurementsModel = new CuttingToolMeasurementsModel();

            var measurements = mtconnectModel.AssetInformationModel.CuttingTools.Classes.Where(o => typeof(MTConnectMeasurementModel).IsAssignableFrom(o.GetType()));
            foreach (var measurement in measurements.OrderBy(o => o.Name)) measurementsModel.Types.Add((MTConnectMeasurementModel)measurement);

            var template = TemplateLoader.LoadOrThrow("Xml", "Templates", "XmlCuttingToolLifeCycle.scriban");
            var result = template.Render(measurementsModel);
            if (result == null) return;

            var resultPath = Path.Combine(outputPath, "Assets/CuttingTools/XmlCuttingToolLifeCycle") + ".g.cs";
            var resultDirectory = Path.GetDirectoryName(resultPath);
            TemplateLoader.EnsureDirectory(resultDirectory);
            File.WriteAllText(resultPath, result);
        }

        private static void WriteCuttingItem(MTConnectModel mtconnectModel, string outputPath)
        {
            var measurementsModel = new CuttingToolMeasurementsModel();

            var measurements = mtconnectModel.AssetInformationModel.CuttingTools.Classes.Where(o => typeof(MTConnectMeasurementModel).IsAssignableFrom(o.GetType()));
            foreach (var measurement in measurements.OrderBy(o => o.Name)) measurementsModel.Types.Add((MTConnectMeasurementModel)measurement);

            var template = TemplateLoader.LoadOrThrow("Xml", "Templates", "XmlCuttingItem.scriban");
            var result = template.Render(measurementsModel);
            if (result == null) return;

            var resultPath = Path.Combine(outputPath, "Assets/CuttingTools/XmlCuttingItem") + ".g.cs";
            var resultDirectory = Path.GetDirectoryName(resultPath);
            TemplateLoader.EnsureDirectory(resultDirectory);
            File.WriteAllText(resultPath, result);
        }
    }
}
