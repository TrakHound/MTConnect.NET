﻿using MTConnect.SysML.Xmi;
using MTConnect.SysML.Xmi.UML;
using Scriban;
using System;
using System.IO;
using System.Linq;

namespace MTConnect.SysML.CSharp
{
    internal class EnumModel : MTConnectEnumModel, ITemplateModel
    {
        public string Namespace => NamespaceHelper.GetNamespace(Id);


        public EnumModel() { }

        public EnumModel(XmiDocument xmiDocument, string id, UmlEnumeration umlEnumeration) : base(xmiDocument, id, umlEnumeration) { }


        public static EnumModel Create(MTConnectEnumModel importModel)
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
                        if (exportProperty != null)
                        {
                            exportProperty.SetValue(exportModel, propertyValue);
                        }
                    }

                    return exportModel;
                }
            }

            return null;
        }


        public string RenderModel()
        {
            var templateFilename = $"Enum.scriban";
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
    }
}
