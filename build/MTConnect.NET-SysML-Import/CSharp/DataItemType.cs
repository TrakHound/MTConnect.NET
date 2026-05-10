using MTConnect.NET_SysML_Import.CSharp;
using MTConnect.SysML.Models.Devices;
using MTConnect.SysML.Xmi;
using MTConnect.SysML.Xmi.UML;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.SysML.CSharp
{
    public class DataItemType : MTConnectDataItemType, ITemplateModel
    {
        public string Namespace => NamespaceHelper.GetNamespace(Id);

        public string XmlDescription { get; set; }

        public string DefaultName => GetName(Type);

        public string UnitsEnum => Units != null ? $"Devices.{Units}" : null;

        public string MaximumVersionEnum => MTConnectVersion.GetVersionEnum(MaximumVersion);

        public string MinimumVersionEnum => MTConnectVersion.GetVersionEnum(MinimumVersion);

        public string ResultType { get; set; }


        public DataItemType() { }

        public DataItemType(XmiDocument xmiDocument, string category, string idPrefix, UmlClass umlClass, UmlEnumerationLiteral umlEnumerationLiteral, IEnumerable<UmlClass> subClasses = null)
            : base (xmiDocument, category, idPrefix, umlClass, umlEnumerationLiteral, subClasses) { }


        public static DataItemType Create(MTConnectDataItemType importModel)
        {
            if (importModel != null)
            {
                var type = typeof(DataItemType);

                var importProperties = importModel.GetType().GetProperties();
                var exportProperties = type.GetProperties();

                if (importProperties != null && exportProperties != null)
                {
                    var exportModel = new DataItemType();

                    foreach (var importProperty in importProperties)
                    {
                        var propertyValue = importProperty.GetValue(importModel);

                        var exportProperty = exportProperties.FirstOrDefault(o => o.Name == importProperty.Name);
                        // Require matching PropertyType so SetValue cannot throw
                        // ArgumentException — a future divergence in property
                        // types between the import and export hierarchies would
                        // otherwise blow up at runtime instead of silently
                        // skipping the mismatched property.
                        if (exportProperty != null && exportProperty.PropertyType == importProperty.PropertyType)
                        {
                            exportProperty.SetValue(exportModel, propertyValue);
                        }
                    }

                    if (exportModel.Units != null)
                    {
                        exportModel.Units = exportModel.Units.Replace("NativeUnitsEnum", "NativeUnits");
                        exportModel.Units = exportModel.Units.Replace("UnitsEnum", "Units");
                        exportModel.Units = UnitsHelper.Get(exportModel.Units);
                    }

                    if (importModel.Result != null)
                    {
                        exportModel.ResultType = ModelHelper.RemoveEnumSuffix(importModel.Result);
                    }

                    // Guard before `+= "DataItem"` so a null Id/Name does not silently yield the literal "DataItem".
                    if (exportModel.Id == null)
                        throw new InvalidOperationException("DataItemType has null Id, cannot append 'DataItem' suffix.");
                    if (exportModel.Name == null)
                        throw new InvalidOperationException($"DataItemType '{exportModel.Id}' has null Name, cannot append 'DataItem' suffix.");
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


        public virtual string RenderModel()
        {
            var template = TemplateLoader.LoadOrThrow("CSharp", "Templates", "Devices.DataItemType.scriban");
            return template.Render(this);
        }

        public string RenderInterface() => null;

        public string RenderDescriptions() => null;


		private static string GetName(string type)
		{
			switch (type)
			{
				case "EMERGENCY_STOP": return "estop";
				case "CONTROLLER_MODE": return "mode";
				case "EXECUTION": return "exec";
				case "LOAD": return "load";
				case "POSITION": return "pos";
				case "TEMPERATURE": return "temp";
			}

			return type.ToCamelCase();
		}
	}
}
