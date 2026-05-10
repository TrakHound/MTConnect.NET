using MTConnect.NET_SysML_Import.CSharp;
using MTConnect.SysML.Xmi;
using MTConnect.SysML.Xmi.UML;
using System;
using System.Linq;

namespace MTConnect.SysML.CSharp
{
    internal class EnumStringModel : MTConnectEnumModel, ITemplateModel
    {
        public string Namespace => NamespaceHelper.GetNamespace(Id);

        public string XmlDescription { get; set; }

        public bool IsPartial { get; set; }


        public EnumStringModel() { }

        public EnumStringModel(XmiDocument xmiDocument, string id, UmlEnumeration umlEnumeration) : base(xmiDocument, id, umlEnumeration) { }


        public static EnumStringModel Create(MTConnectEnumModel importModel, Func<string, string> convertFunction = null)
        {
            if (importModel != null)
            {
                var type = typeof(EnumStringModel);

                var importProperties = importModel.GetType().GetProperties();
                var exportProperties = type.GetProperties();

                if (importProperties != null && exportProperties != null)
                {
                    var exportModel = new EnumStringModel();

                    foreach (var importProperty in importProperties)
                    {
                        var propertyValue = importProperty.GetValue(importModel);

                        var exportProperty = exportProperties.FirstOrDefault(o => o.Name ==  importProperty.Name);
                        if (exportProperty != null && exportProperty.PropertyType == importProperty.PropertyType)
                        {
                            exportProperty.SetValue(exportModel, propertyValue);
                        }
                    }

                    exportModel.Id = ModelHelper.RemoveEnumSuffix(exportModel.Id);
                    exportModel.Name = ModelHelper.RemoveEnumSuffix(exportModel.Name);
                    exportModel.Description = DescriptionHelper.GetTextDescription(importModel.Description);
                    exportModel.XmlDescription = DescriptionHelper.GetXmlDescription(importModel.Description);

                    if (exportModel.Values != null)
                    {
                        foreach (var value in exportModel.Values)
                        {
                            if (convertFunction != null)
                            {
                                value.Name = convertFunction(value.Name);
                            }
                            else
                            {
                                // Convert Numbers to Strings (leading numbers aren't supported as Enum values)
                                var name = value.Name;
                                name = StringFunctions.ReplaceNumbersWithWords(name);
                                value.Name = name.ToTitleCase();

                                // Replace '/' (not supported as Enum values)
                                value.Name = value.Name.Replace('/', '_');
                            }
                        }
                    }

                    return exportModel;
                }
            }

            return null;
        }


        public string RenderModel()
        {
            var template = TemplateLoader.LoadOrThrow("CSharp", "Templates", "EnumString.scriban");
            return template.Render(this);
        }

        public string RenderInterface() => null;

        public string RenderDescriptions()
        {
            var template = TemplateLoader.LoadOrThrow("CSharp", "Templates", "EnumStringDescriptions.scriban");
            return template.Render(this);
        }
    }
}
