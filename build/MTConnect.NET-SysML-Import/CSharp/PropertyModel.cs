using MTConnect.NET_SysML_Import.CSharp;
using MTConnect.SysML.Xmi;
using MTConnect.SysML.Xmi.UML;
using System.Linq;

namespace MTConnect.SysML.CSharp
{
    /// <summary>
    /// Template model for a single C# property on a generated class.
    /// Carries emitter flags (<see cref="ExportToModel"/> and
    /// <see cref="ExportToInterface"/>) so a property can be elided
    /// from the model or the interface independently.
    /// </summary>
    public class PropertyModel : MTConnectPropertyModel
    {
        /// <summary>XML-formatted description (XML doc-comment shape).</summary>
        public string XmlDescription { get; set; }

        /// <summary>
        /// When <c>true</c> (default), the property is emitted into
        /// the generated concrete class.
        /// </summary>
        public bool ExportToModel { get; set; } = true;

        /// <summary>
        /// When <c>true</c> (default), the property is emitted into
        /// the generated interface as well.
        /// </summary>
        public bool ExportToInterface { get; set; } = true;


        /// <summary>Parameterless constructor used by the reflection-
        /// based <see cref="Create"/> factory.</summary>
        public PropertyModel() { }

        /// <summary>
        /// Constructs a model directly from a UML property.
        /// </summary>
        /// <param name="xmiDocument">Source XMI document.</param>
        /// <param name="id">Identifier of the parent type.</param>
        /// <param name="umlProperty">Backing UML property.</param>
        public PropertyModel(XmiDocument xmiDocument, string id, UmlProperty umlProperty) : base(xmiDocument, id, umlProperty) { }


        /// <summary>
        /// Copies every matching property off <paramref name="importModel"/>
        /// into a fresh <see cref="PropertyModel"/>. Returns <c>null</c>
        /// when the input is null.
        /// </summary>
        /// <param name="importModel">Generic SysML-import model.</param>
        /// <returns>Emitter-aware model, or <c>null</c>.</returns>
        public static PropertyModel Create(MTConnectPropertyModel importModel)
        {
            if (importModel != null)
            {
                var type = typeof(PropertyModel);

                var importProperties = importModel.GetType().GetProperties();
                var exportProperties = type.GetProperties();

                if (importProperties != null && exportProperties != null)
                {
                    var exportModel = new PropertyModel();

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
    }
}
