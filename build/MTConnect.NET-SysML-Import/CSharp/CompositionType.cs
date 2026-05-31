using MTConnect.NET_SysML_Import.CSharp;
using MTConnect.SysML.Models.Devices;
using MTConnect.SysML.Xmi;
using MTConnect.SysML.Xmi.UML;

namespace MTConnect.SysML.CSharp
{
    /// <summary>
    /// Template model for an MTConnect <c>CompositionType</c>. Same
    /// shape as <see cref="ComponentType"/> but backed by a SysML
    /// enumeration literal rather than a UML class.
    /// </summary>
    public class CompositionType : MTConnectCompositionType, ITemplateModel
    {
        /// <summary>C# namespace the generated type belongs to.</summary>
        public string Namespace => NamespaceHelper.GetNamespace(Id);

        /// <summary>XML-formatted description (XML doc-comment shape).</summary>
        public string XmlDescription { get; set; }

        /// <summary>SysML <c>MaximumVersion</c> mapped to a C# enum value.</summary>
        public string MaximumVersionEnum => MTConnectVersion.GetVersionEnum(MaximumVersion);

        /// <summary>SysML <c>MinimumVersion</c> mapped to a C# enum value.</summary>
        public string MinimumVersionEnum => MTConnectVersion.GetVersionEnum(MinimumVersion);


        /// <summary>Parameterless constructor used by the reflection-based
        /// <see cref="Create"/> factory.</summary>
        public CompositionType() { }

        /// <summary>
        /// Constructs a model directly from an XMI document tree.
        /// </summary>
        /// <param name="xmiDocument">Source XMI document.</param>
        /// <param name="idPrefix">Identifier prefix applied to the
        /// rendered type.</param>
        /// <param name="umlEnumerationLiteral">Backing UML enumeration
        /// literal.</param>
        public CompositionType(XmiDocument xmiDocument, string idPrefix, UmlEnumerationLiteral umlEnumerationLiteral) : base (xmiDocument, idPrefix, umlEnumerationLiteral) { }


        /// <summary>
        /// Copies every matching property off <paramref name="importModel"/>
        /// into a fresh <see cref="CompositionType"/>. Returns
        /// <c>null</c> when the input is null.
        /// </summary>
        /// <param name="importModel">Generic SysML-import model.</param>
        /// <returns>Emitter-aware model, or <c>null</c>.</returns>
        public static CompositionType Create(MTConnectCompositionType importModel)
        {
            if (importModel != null)
            {
                var type = typeof(CompositionType);

                var importProperties = importModel.GetType().GetProperties();
                var exportProperties = type.GetProperties();

                if (importProperties != null && exportProperties != null)
                {
                    var exportModel = new CompositionType();

                    foreach (var importProperty in importProperties)
                    {
                        var propertyValue = importProperty.GetValue(importModel);

                        var exportProperty = exportProperties.FirstOrDefault(o => o.Name == importProperty.Name);
                        // Require matching PropertyType so SetValue cannot throw
                        // ArgumentException when a property of the same name has
                        // a different declared type on the export model.
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
            var template = TemplateLoader.LoadOrThrow("CSharp", "Templates", "Devices.CompositionType.scriban");
            return template.Render(this);
        }

        /// <inheritdoc />
        public string RenderInterface() => null;

        /// <inheritdoc />
        public string RenderDescriptions() => null;
    }
}
