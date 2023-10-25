using MTConnect.SysML.Xmi;
using MTConnect.SysML.Xmi.UML;
using Scriban;
using System;
using System.IO;
using System.Linq;

namespace MTConnect.SysML.CSharp
{
    internal class EnumStringModel : MTConnectEnumModel, ITemplateModel
    {
        public string Namespace => NamespaceHelper.GetNamespace(Id);


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
                        if (exportProperty != null)
                        {
                            exportProperty.SetValue(exportModel, propertyValue);
                        }
                    }

                    exportModel.Id = ModelHelper.RemoveEnumSuffix(exportModel.Id);
                    exportModel.Name = ModelHelper.RemoveEnumSuffix(exportModel.Name);

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
            var templateFilename = $"EnumString.scriban";
            var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "csharp", "templates", templateFilename);
            if (File.Exists(templatePath))
            {
                try
                {
                    var templateContents = File.ReadAllText(templatePath);
                    if (templateContents != null)
                    {
                        var template = Template.Parse(templateContents);
                        return template.Render(this);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return null;
        }

        public string RenderInterface() => null;

        public string RenderDescriptions()
        {
            var templateFilename = $"EnumDescriptions.scriban";
            var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "csharp", "templates", templateFilename);
            if (File.Exists(templatePath))
            {
                try
                {
                    var templateContents = File.ReadAllText(templatePath);
                    if (templateContents != null)
                    {
                        var template = Template.Parse(templateContents);
                        return template.Render(this);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return null;
        }
    }
}
