using MTConnect.NET_SysML_Import.CSharp;
using MTConnect.SysML.Xmi;
using MTConnect.SysML.Xmi.UML;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.SysML.CSharp
{
    /// <summary>
    /// Template model for a generic SysML class rendered via
    /// <c>Model.scriban</c> / <c>Interface.scriban</c> /
    /// <c>ModelDescriptions.scriban</c>. Sibling to
    /// <see cref="ComponentType"/> / <see cref="DataItemType"/> in the
    /// export-model family — kept <c>public</c> so parity tests can
    /// construct instances directly and exercise the render pipeline.
    /// </summary>
    public class ClassModel : MTConnectClassModel, ITemplateModel
    {
        /// <summary>
        /// C# namespace the generated type belongs to, derived from
        /// <see cref="MTConnectClassModel.Id"/> via
        /// <see cref="NamespaceHelper.GetNamespace"/>.
        /// </summary>
        public string Namespace => NamespaceHelper.GetNamespace(Id);

        /// <summary>
        /// XML-formatted description (XML doc-comment shape) emitted into
        /// the descriptions Scriban template.
        /// </summary>
        public string XmlDescription { get; set; }

        /// <summary>
        /// <c>true</c> when the emitted class is declared <c>partial</c>
        /// so hand-written companion partials can extend it.
        /// </summary>
        public bool IsPartial { get; set; }

        /// <summary>
        /// <c>true</c> to render the class body via
        /// <see cref="RenderModel"/>. Set to <c>false</c> for
        /// interface-only or descriptions-only exports.
        /// </summary>
        public bool HasModel { get; set; } = true;

        /// <summary>
        /// <c>true</c> to render the sibling interface via
        /// <see cref="RenderInterface"/>.
        /// </summary>
        public bool HasInterface { get; set; } = true;

        /// <summary>
        /// <c>true</c> to render the descriptions lookup class via
        /// <see cref="RenderDescriptions"/>.
        /// </summary>
        public bool HasDescriptions { get; set; } = true;

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
        /// Emitter-aware property list — shadows the base
        /// <see cref="MTConnectClassModel.Properties"/> with
        /// <see cref="PropertyModel"/> entries that expose the C#-only
        /// flags (<c>IsInherited</c>, <c>IsInheritedInInterface</c>,
        /// <c>ExportToInterface</c>).
        /// </summary>
        public new List<PropertyModel> Properties { get; set; } = new();

        /// <summary>
        /// <c>true</c> when at least one ancestor in the
        /// <see cref="MTConnectClassModel.ParentName"/> chain declares a
        /// non-empty <see cref="MTConnectClassModel.Rules"/> array.
        /// <c>Model.scriban</c> uses this — rather than mere parent
        /// presence — to decide whether the generated <c>Rules</c> field
        /// needs the <c>new</c> modifier. A class can have a parent
        /// without that parent (or any of its ancestors) declaring
        /// <c>Rules</c>, in which case emitting <c>new</c> hides nothing
        /// and the compiler raises CS0109. Populated by
        /// <see cref="CSharpTemplateRenderer"/> after every
        /// <see cref="ClassModel"/> has been assembled, so the full
        /// ancestor chain is resolvable.
        /// </summary>
        public bool ParentHasRules { get; set; }


        /// <summary>
        /// Parameterless constructor used by the import pipeline when it
        /// copies properties off an existing <see cref="MTConnectClassModel"/>
        /// via reflection.
        /// </summary>
        public ClassModel() { }

        /// <summary>
        /// Constructs a model directly from an XMI document tree by
        /// delegating to the base type's constructor.
        /// </summary>
        /// <param name="xmiDocument">Source XMI document.</param>
        /// <param name="id">Identifier prefix applied to the rendered
        /// type.</param>
        /// <param name="umlClass">Backing UML class.</param>
        public ClassModel(XmiDocument xmiDocument, string id, UmlClass umlClass) : base(xmiDocument, id, umlClass) { }


        /// <summary>
        /// Copies every matching property off <paramref name="importModel"/>
        /// into a fresh <see cref="ClassModel"/>, translating property
        /// entries via <see cref="PropertyModel.Create"/>. Returns
        /// <c>null</c> when the input is <c>null</c>.
        /// </summary>
        /// <param name="importModel">Generic SysML-import model.</param>
        /// <returns>Emitter-aware model, or <c>null</c>.</returns>
        public static ClassModel Create(MTConnectClassModel importModel)
        {
            if (importModel != null)
            {
                var type = typeof(ClassModel);

                var importProperties = importModel.GetType().GetProperties();
                var exportProperties = type.GetProperties();

                if (importProperties != null && exportProperties != null)
                {
                    var exportModel = new ClassModel();

                    foreach (var importProperty in importProperties)
                    {
                        var propertyValue = importProperty.GetValue(importModel);

                        var exportProperty = exportProperties.FirstOrDefault(o => o.Name == importProperty.Name);
                        if (exportProperty != null && exportProperty.PropertyType == importProperty.PropertyType)
                        {
                            exportProperty.SetValue(exportModel, propertyValue);
                        }
                    }

                    foreach (var propertyModel in importModel.Properties)
                    {
                        var exportPropertyModel = PropertyModel.Create(propertyModel);

                        // Remove 'Enum' suffix
                        if (exportPropertyModel.DataType.EndsWith("Enum"))
                        {
                            var suffix = "Enum";
                            if (exportPropertyModel.DataType.EndsWith(suffix)) exportPropertyModel.DataType = exportPropertyModel.DataType.Substring(0, exportPropertyModel.DataType.Length - suffix.Length);
                        }

                        exportModel.Properties.Add(exportPropertyModel);
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
            if (!HasModel) return null;
            var template = TemplateLoader.LoadOrThrow("CSharp", "Templates", "Model.scriban");
            return template.Render(this);
        }

        /// <inheritdoc />
        public string RenderInterface()
        {
            if (!HasInterface) return null;
            var template = TemplateLoader.LoadOrThrow("CSharp", "Templates", "Interface.scriban");
            return template.Render(this);
        }

        /// <inheritdoc />
        public string RenderDescriptions()
        {
            if (Properties == null || Properties.Count == 0) return null;
            if (!HasDescriptions) return null;
            var template = TemplateLoader.LoadOrThrow("CSharp", "Templates", "ModelDescriptions.scriban");
            return template.Render(this);
        }
    }
}
