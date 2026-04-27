using MTConnect.NET_SysML_Import.CSharp;
using MTConnect.SysML.Xmi;
using MTConnect.SysML.Xmi.UML;
using System;
using System.Linq;

namespace MTConnect.SysML.CSharp
{
    internal class EnumModel : MTConnectEnumModel, ITemplateModel
    {
        public string Namespace => NamespaceHelper.GetNamespace(Id);

        public string XmlDescription { get; set; }


        public EnumModel() { }

        public EnumModel(XmiDocument xmiDocument, string id, UmlEnumeration umlEnumeration) : base(xmiDocument, id, umlEnumeration) { }


        public static EnumModel Create(MTConnectEnumModel importModel, Func<string, string> convertFunction = null)
        {
            if (importModel != null)
            {
                var type = typeof(EnumModel);

                var importProperties = importModel.GetType().GetProperties();
                var exportProperties = type.GetProperties();

                if (importProperties != null && exportProperties != null)
                {
                    var exportModel = new EnumModel();

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
                                var name = value.Name?.ToUnderscoreUpper();
                                name = StringFunctions.ReplaceNumbersWithWords(name);

                                // Replace '/' (not supported as Enum values)
                                name = name.Replace('/', '_');

                                name = name.ToUnderscoreUpper();
                                value.Name = name;
                            }

                            //value.Name = value.Name.Replace("/", "_PER_");
                            //value.Name = value.Name.Replace("^2", "_SQUARED");

                            //// Convert Numbers to Strings (leading numbers aren't supported as Enum values)
                            //var name = value.Name;
                            //name = StringFunctions.ReplaceNumbersWithWords(name);
                            //value.Name = name.ToTitleCase();

                            //// Replace '/' (not supported as Enum values)
                            //value.Name = value.Name.Replace('/', '_');
                        }
                    }

                    return exportModel;
                }
            }

            return null;
        }


        public string RenderModel()
        {
            var template = TemplateLoader.LoadOrThrow("CSharp", "Templates", "Enum.scriban");
            return template.Render(this);
        }

        public string RenderInterface() => null;

        public string RenderDescriptions()
        {
            if (Values == null || Values.Count == 0) return null;
            var template = TemplateLoader.LoadOrThrow("CSharp", "Templates", "EnumDescriptions.scriban");
            return template.Render(this);
        }
    }
}
