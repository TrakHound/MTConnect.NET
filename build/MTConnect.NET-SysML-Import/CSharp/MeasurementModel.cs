using MTConnect.NET_SysML_Import.CSharp;
using MTConnect.SysML.Models.Assets;

namespace MTConnect.SysML.CSharp
{
    /// <summary>
    /// Template model for an MTConnect pallet-asset measurement.
    /// Sibling to <see cref="CuttingToolMeasurementModel"/>; uses the
    /// <c>Pallets.Measurement.scriban</c> template instead.
    /// </summary>
    public class MeasurementModel : MTConnectMeasurementModel, ITemplateModel
    {
        /// <summary>C# namespace the generated type belongs to.</summary>
        public string Namespace => NamespaceHelper.GetNamespace(Id);

        /// <summary>XML-formatted description (XML doc-comment shape).</summary>
        public string XmlDescription { get; set; }


        /// <summary>
        /// Copies every matching property off <paramref name="importModel"/>
        /// into a fresh <see cref="MeasurementModel"/>. Returns
        /// <c>null</c> when the input is null.
        /// </summary>
        /// <param name="importModel">Generic SysML-import model.</param>
        /// <returns>Emitter-aware model, or <c>null</c>.</returns>
        public static MeasurementModel Create(MTConnectMeasurementModel importModel)
        {
            if (importModel != null)
            {
                var type = typeof(MeasurementModel);

                var importProperties = importModel.GetType().GetProperties();
                var exportProperties = type.GetProperties();

                if (importProperties != null && exportProperties != null)
                {
                    var exportModel = new MeasurementModel();

                    foreach (var importProperty in importProperties)
                    {
                        var propertyValue = importProperty.GetValue(importModel);

                        var exportProperty = exportProperties.FirstOrDefault(o => o.Name == importProperty.Name);
                        if (exportProperty != null && exportProperty.PropertyType == importProperty.PropertyType)
                        {
                            exportProperty.SetValue(exportModel, propertyValue);
                        }
                    }

                    exportModel.Description = DescriptionHelper.GetTextDescription(importModel.Description);
                    exportModel.XmlDescription = DescriptionHelper.GetXmlDescription(importModel.Description);

                    return exportModel;
                }
            }

            return null;
        }

        /// <inheritdoc />
        public string RenderModel()
        {
            var template = TemplateLoader.LoadOrThrow("CSharp", "Templates", "Pallets.Measurement.scriban");
            return template.Render(this);
        }

        /// <inheritdoc />
        public string RenderInterface() => null;

        /// <inheritdoc />
        public string RenderDescriptions() => null;
    }
}
