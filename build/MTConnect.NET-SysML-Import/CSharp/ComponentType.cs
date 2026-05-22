using MTConnect.NET_SysML_Import.CSharp;
using MTConnect.SysML.Models.Devices;
using MTConnect.SysML.Xmi;
using MTConnect.SysML.Xmi.UML;

namespace MTConnect.SysML.CSharp
{
    /// <summary>
    /// Template model for an MTConnect <c>ComponentType</c>. Wraps the
    /// SysML-import <see cref="MTConnectComponentType"/> with the
    /// C#-emitter-only properties needed by the Scriban template
    /// (namespace, XML description, version enums) and exposes the
    /// <see cref="ITemplateModel"/> render hooks.
    /// </summary>
    public class ComponentType : MTConnectComponentType, ITemplateModel
    {
        /// <summary>
        /// C# namespace the generated type belongs to. Derived from
        /// the SysML identifier via <see cref="NamespaceHelper"/>.
        /// </summary>
        public string Namespace => NamespaceHelper.GetNamespace(Id);

        /// <summary>
        /// XML-formatted description (XML doc-comment shape) emitted
        /// directly into the Scriban template.
        /// </summary>
        public string XmlDescription { get; set; }

        /// <summary>
        /// SysML <c>MaximumVersion</c> mapped to the C# enum value
        /// emitted by <see cref="MTConnectVersion.GetVersionEnum"/>.
        /// </summary>
        public string MaximumVersionEnum => MTConnectVersion.GetVersionEnum(MaximumVersion);

        /// <summary>
        /// SysML <c>MinimumVersion</c> mapped to the C# enum value
        /// emitted by <see cref="MTConnectVersion.GetVersionEnum"/>.
        /// </summary>
        public string MinimumVersionEnum => MTConnectVersion.GetVersionEnum(MinimumVersion);


        /// <summary>
        /// Parameterless constructor used by the import pipeline when
        /// it copies properties off an existing
        /// <see cref="MTConnectComponentType"/> via reflection.
        /// </summary>
        public ComponentType() { }

        /// <summary>
        /// Constructs a model directly from an XMI document tree by
        /// delegating to the base type's constructor.
        /// </summary>
        /// <param name="xmiDocument">Source XMI document.</param>
        /// <param name="idPrefix">Identifier prefix applied to the
        /// rendered type.</param>
        /// <param name="umlClass">Backing UML class.</param>
        public ComponentType(XmiDocument xmiDocument, string idPrefix, UmlClass umlClass) : base (xmiDocument, idPrefix, umlClass) { }


        /// <summary>
        /// Copies every matching property off <paramref name="importModel"/>
        /// into a fresh <see cref="ComponentType"/>. Returns <c>null</c>
        /// when the input is null. Used by the SysML import pipeline to
        /// convert generic models into emitter-aware models.
        /// </summary>
        /// <param name="importModel">Generic SysML-import model.</param>
        /// <returns>Emitter-aware model, or <c>null</c>.</returns>
        public static ComponentType Create(MTConnectComponentType importModel)
        {
            if (importModel != null)
            {
                var type = typeof(ComponentType);

                var importProperties = importModel.GetType().GetProperties();
                var exportProperties = type.GetProperties();

                if (importProperties != null && exportProperties != null)
                {
                    var exportModel = new ComponentType();

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
            var template = TemplateLoader.LoadOrThrow("CSharp", "Templates", "Devices.ComponentType.scriban");
            return template.Render(this);
        }

        /// <inheritdoc />
        public string RenderInterface() => null;

        /// <inheritdoc />
        public string RenderDescriptions() => null;
    }
}
