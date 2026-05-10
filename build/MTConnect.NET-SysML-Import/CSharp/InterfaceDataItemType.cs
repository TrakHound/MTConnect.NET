using MTConnect.NET_SysML_Import.CSharp;
using MTConnect.SysML.Models.Devices;
using MTConnect.SysML.Xmi;
using MTConnect.SysML.Xmi.UML;
using System;
using System.Collections.Generic;
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
                        // Require matching PropertyType so SetValue cannot throw
                        // ArgumentException when a property of the same name has
                        // a different declared type on the export model.
                        if (exportProperty != null && exportProperty.PropertyType == importProperty.PropertyType)
                        {
                            exportProperty.SetValue(exportModel, propertyValue);
                        }
                    }

                    // Guard before `+= "DataItem"` so a null Id/Name does not silently yield the literal "DataItem".
                    if (exportModel.Id == null)
                        throw new InvalidOperationException("InterfaceDataItemType has null Id, cannot append 'DataItem' suffix.");
                    if (exportModel.Name == null)
                        throw new InvalidOperationException($"InterfaceDataItemType '{exportModel.Id}' has null Name, cannot append 'DataItem' suffix.");
                    exportModel.Id += "DataItem";
                    exportModel.Name += "DataItem";
                    exportModel.Description = DescriptionHelper.GetTextDescription(importModel.Description);
                    exportModel.XmlDescription = DescriptionHelper.GetXmlDescription(importModel.Description);
                    if (exportModel.ParentName != null && exportModel.ParentName != "DataItem") exportModel.ParentName += "DataItem";

                    return exportModel;
                }
            }

            return null;
        }

        public override string RenderModel()
        {
            var template = TemplateLoader.LoadOrThrow("CSharp", "Templates", "Interfaces.InterfaceDataItemType.scriban");
            return template.Render(this);
        }

        public string RenderInterface() => null;

        public string RenderDescriptions() => null;
    }
}
