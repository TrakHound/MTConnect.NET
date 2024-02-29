﻿using MTConnect.NET_SysML_Import.CSharp;
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
                        if (exportProperty != null)
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
            var templateFilename = $"Devices.DataItemType.scriban";
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
