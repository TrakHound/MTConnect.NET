using MTConnect.NET_SysML_Import.CSharp;
using MTConnect.SysML.Models.Devices;
using MTConnect.SysML.Xmi;
using MTConnect.SysML.Xmi.UML;

namespace MTConnect.SysML.CSharp
{
    public class CompositionType : MTConnectCompositionType, ITemplateModel
    {
        public string Namespace => NamespaceHelper.GetNamespace(Id);

        public string XmlDescription { get; set; }

        public string MaximumVersionEnum => MTConnectVersion.GetVersionEnum(MaximumVersion);

        public string MinimumVersionEnum => MTConnectVersion.GetVersionEnum(MinimumVersion);


        public CompositionType() { }

        public CompositionType(XmiDocument xmiDocument, string idPrefix, UmlEnumerationLiteral umlEnumerationLiteral) : base (xmiDocument, idPrefix, umlEnumerationLiteral) { }


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


        public string RenderModel()
        {
            var template = TemplateLoader.LoadOrThrow("CSharp", "Templates", "Devices.CompositionType.scriban");
            return template.Render(this);
        }

        public string RenderInterface() => null;

        public string RenderDescriptions() => null;
    }
}
