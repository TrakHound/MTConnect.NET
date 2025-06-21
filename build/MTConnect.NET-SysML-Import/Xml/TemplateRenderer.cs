using MTConnect.SysML.Models.Assets;
using Scriban;

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

            var templateFilename = $"XmlMeasurements.scriban";
            var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "xml", "templates", templateFilename);
            if (File.Exists(templatePath))
            {
                try
                {
                    var templateContents = File.ReadAllText(templatePath);
                    if (templateContents != null)
                    {
                        var template = Template.Parse(templateContents);
                        var result = template.Render(measurementsModel);
                        if (result != null)
                        {
                            var resultPath = $"Assets/CuttingTools/XmlMeasurements";
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

        private static void WriteCuttingToolLifeCycle(MTConnectModel mtconnectModel, string outputPath)
        {
            var measurementsModel = new CuttingToolMeasurementsModel();

            var measurements = mtconnectModel.AssetInformationModel.CuttingTools.Classes.Where(o => typeof(MTConnectMeasurementModel).IsAssignableFrom(o.GetType()));
            foreach (var measurement in measurements.OrderBy(o => o.Name)) measurementsModel.Types.Add((MTConnectMeasurementModel)measurement);

            var templateFilename = $"XmlCuttingToolLifeCycle.scriban";
            var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "xml", "templates", templateFilename);
            if (File.Exists(templatePath))
            {
                try
                {
                    var templateContents = File.ReadAllText(templatePath);
                    if (templateContents != null)
                    {
                        var template = Template.Parse(templateContents);
                        var result = template.Render(measurementsModel);
                        if (result != null)
                        {
                            var resultPath = $"Assets/CuttingTools/XmlCuttingToolLifeCycle";
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

        private static void WriteCuttingItem(MTConnectModel mtconnectModel, string outputPath)
        {
            var measurementsModel = new CuttingToolMeasurementsModel();

            var measurements = mtconnectModel.AssetInformationModel.CuttingTools.Classes.Where(o => typeof(MTConnectMeasurementModel).IsAssignableFrom(o.GetType()));
            foreach (var measurement in measurements.OrderBy(o => o.Name)) measurementsModel.Types.Add((MTConnectMeasurementModel)measurement);

            var templateFilename = $"XmlCuttingItem.scriban";
            var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "xml", "templates", templateFilename);
            if (File.Exists(templatePath))
            {
                try
                {
                    var templateContents = File.ReadAllText(templatePath);
                    if (templateContents != null)
                    {
                        var template = Template.Parse(templateContents);
                        var result = template.Render(measurementsModel);
                        if (result != null)
                        {
                            var resultPath = $"Assets/CuttingTools/XmlCuttingItem";
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
