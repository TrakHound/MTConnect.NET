using MTConnect.NET_SysML_Import.CSharp;
using MTConnect.SysML.Models.Assets;
using MTConnect.SysML.Models.Observations;
using System.Linq;

namespace MTConnect.SysML.CSharp
{
    /// <summary>
    /// Template model for an MTConnect <c>DataSet</c> result-type
    /// entry. Renders the keys-and-values shape used by Sample / Event
    /// data items whose result type is a DataSet.
    /// </summary>
    public class DataSetResultModel : MTConnectClassModel, ITemplateModel
    {
        /// <summary>C# namespace the generated type belongs to.</summary>
        public string Namespace => NamespaceHelper.GetNamespace(Id);

        /// <summary>XML-formatted description (XML doc-comment shape).</summary>
        public string XmlDescription { get; set; }


        /// <summary>
        /// Copies every matching property off <paramref name="importModel"/>
        /// into a fresh <see cref="DataSetResultModel"/> and strips the
        /// SysML <c>Enum</c> suffix from every property's
        /// <c>DataType</c>. Returns <c>null</c> when the input is null.
        /// </summary>
        /// <param name="importModel">Generic SysML-import model.</param>
        /// <returns>Emitter-aware model, or <c>null</c>.</returns>
        public static DataSetResultModel Create(MTConnectClassModel importModel)
        {
            if (importModel != null)
            {
                // Use the export type (DataSetResultModel) so reflection picks up
                // the export-side properties; the previous `typeof(MTConnectClassModel)`
                // pointed at the parent and silently dropped DataSetResult-specific
                // properties.
                var type = typeof(DataSetResultModel);

                var importProperties = importModel.GetType().GetProperties();
                var exportProperties = type.GetProperties();

                if (importProperties != null && exportProperties != null)
                {
                    var exportModel = new DataSetResultModel();

                    foreach (var importProperty in importProperties)
                    {
                        var propertyValue = importProperty.GetValue(importModel);

                        var exportProperty = exportProperties.FirstOrDefault(o => o.Name == importProperty.Name);
                        if (exportProperty != null && exportProperty.PropertyType == importProperty.PropertyType)
                        {
                            exportProperty.SetValue(exportModel, propertyValue);
                        }
                    }

                    foreach (var exportProperty in exportModel.Properties)
                    {
                        exportProperty.DataType = ModelHelper.RemoveEnumSuffix(exportProperty.DataType);
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
            var template = TemplateLoader.LoadOrThrow("CSharp", "Templates", "Observations.DataSetResults.scriban");
            return template.Render(this);
        }

        /// <inheritdoc />
        public string RenderInterface() => null;

        /// <inheritdoc />
        public string RenderDescriptions() => null;
    }
}
