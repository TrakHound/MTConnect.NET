using MTConnect.SysML.Models.Devices;
using MTConnect.SysML.Xmi;
using MTConnect.SysML.Xmi.UML;
using Scriban;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MTConnect.SysML.CSharp
{
    public class InterfaceDataItemType : DataItemType
    {
        public InterfaceDataItemType() { }

        public InterfaceDataItemType(XmiDocument xmiDocument, string category, string idPrefix, UmlClass umlClass, UmlEnumerationLiteral umlEnumerationLiteral, IEnumerable<UmlClass> subClasses = null)
            : base (xmiDocument, category, idPrefix, umlClass, umlEnumerationLiteral, subClasses) { }


        public static InterfaceDataItemType Create(MTConnectInterfaceDataItemType importModel)
        {
            if (importModel != null)
            {
                var type = typeof(InterfaceDataItemType);

                var importProperties = importModel.GetType().GetProperties();
                var exportProperties = type.GetProperties();

                if (importProperties != null && exportProperties != null)
                {
                    var exportModel = new InterfaceDataItemType();

                    foreach (var importProperty in importProperties)
                    {
                        var propertyValue = importProperty.GetValue(importModel);

                        var exportProperty = exportProperties.FirstOrDefault(o => o.Name == importProperty.Name);
                        if (exportProperty != null)
                        {
                            exportProperty.SetValue(exportModel, propertyValue);
                        }
                    }

                    exportModel.Id += "DataItem";
                    exportModel.Name += "DataItem";
                    if (exportModel.ParentName != null && exportModel.ParentName != "DataItem") exportModel.ParentName += "DataItem";

                    return exportModel;
                }
            }

            return null;
        }

        public override string RenderModel()
        {
            var templateFilename = $"Interfaces.InterfaceDataItemType.scriban";
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

        public string RenderDescriptions() => null;
    }
}
