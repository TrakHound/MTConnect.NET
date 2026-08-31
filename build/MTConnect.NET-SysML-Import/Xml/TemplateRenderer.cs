using MTConnect.SysML.Models.Assets;

namespace MTConnect.SysML.Xml
{
    /// <summary>
    /// Orchestrator that renders the XML-emitter <c>*.g.cs</c>
    /// artefacts (cutting-tool measurements + life-cycle + cutting
    /// item) under <c>libraries/MTConnect.NET-XML/</c> from a SysML
    /// import model.
    /// </summary>
    public static class XmlTemplateRenderer
    {
        /// <summary>
        /// Renders every XML artefact from the supplied SysML import
        /// model.
        /// </summary>
        /// <param name="mtconnectModel">Fully-loaded SysML import
        /// model.</param>
        /// <param name="outputPath">Repository root the
        /// <c>libraries/MTConnect.NET-XML/</c> subtree is written
        /// into.</param>
        public static void Render(MTConnectModel mtconnectModel, string outputPath)
        {
            if (mtconnectModel != null && !string.IsNullOrEmpty(outputPath))
            {
                // The one CuttingToolMeasurementsModel drives every Xml artefact.
                // The XmlMeasurements.scriban template emits the per-measurement
                // Xml<Name> wrapper subclasses; the shared Shape-A host template
                // (XmlMeasurementArrayHost.scriban) emits both partial-class
                // artefacts (XmlCuttingToolLifeCycle + XmlCuttingItem), each with
                // its own class name and doc-summary values. Consolidating the
                // two per-host templates into one keeps emission byte-identical.
                var measurementsModel = BuildCuttingToolMeasurementsModel(mtconnectModel);

                RenderTo("XmlMeasurements.scriban", measurementsModel, "Assets/CuttingTools/XmlMeasurements", outputPath);

                var arrayHosts = new (string ClassName, string Summary, string OutputRelative)[]
                {
                    (
                        "XmlCuttingToolLifeCycle",
                        "The set of physical and geometric measurements that characterize the cutting tool\n        /// over its life cycle. Each element is deserialized into the concrete\n        /// <see cref=\"XmlMeasurement\"/> subclass registered for its MTConnect measurement type.",
                        "Assets/CuttingTools/XmlCuttingToolLifeCycle"
                    ),
                    (
                        "XmlCuttingItem",
                        "The set of physical and geometric measurements that characterize this cutting item.\n        /// Each element is deserialized into the concrete <see cref=\"XmlMeasurement\"/> subclass\n        /// registered for its MTConnect measurement type.",
                        "Assets/CuttingTools/XmlCuttingItem"
                    ),
                };
                foreach (var (className, summary, output) in arrayHosts)
                {
                    // Anonymous model — Scriban resolves properties by snake_case
                    // convention, so ClassName → class_name, Summary → summary,
                    // Types → types.
                    var hostModel = new
                    {
                        class_name = className,
                        summary = summary,
                        types = measurementsModel.Types
                    };
                    RenderTo("XmlMeasurementArrayHost.scriban", hostModel, output, outputPath);
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
