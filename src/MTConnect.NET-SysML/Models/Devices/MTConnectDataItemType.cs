using MTConnect.SysML.Xmi;
using MTConnect.SysML.Xmi.UML;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.SysML.Models.Devices
{
    public class MTConnectDataItemType : IMTConnectExportModel
    {
        public string UmlId { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }

        public string Type { get; set; }

        public string ParentName { get; set; }

        public string Representation { get; set; }

        public string Units { get; set; }

        public string DefaultSubType { get; set; }

        public string Result { get; set; }

        public Version MaximumVersion { get; set; }

        public Version MinimumVersion { get; set; }

        public List<MTConnectDataItemSubType> SubTypes { get; set; }


        public MTConnectDataItemType() { }

        public MTConnectDataItemType(XmiDocument xmiDocument, string category, string idPrefix, UmlClass umlClass, UmlEnumerationLiteral umlEnumerationLiteral, IEnumerable<UmlClass> subClasses = null)
        {
            if (umlClass != null && umlEnumerationLiteral != null)
            {
                UmlId = umlClass.Id;

                var name = $"{ConvertClassName(umlClass.Name)}DataItem";

                //string name;
                //switch (umlClass.Name)
                //{
                //    case "AdapterURI": name = "AdapterUriDataItem"; break;
                //    case "MTConnectVersion": name = "MTConnectVersionDataItem"; break;
                //    default: name = $"{umlClass.Name.ToTitleCase()}DataItem"; break;
                //}

                Id = $"{idPrefix}.{name}";
                Name = name;
                Category = category;
                Type = umlEnumerationLiteral.Name;
                //Type = umlClass.Name.ToUnderscoreUpper();

                var description = umlEnumerationLiteral.Comments?.FirstOrDefault().Body;
                Description = ModelHelper.ProcessDescription(description);

                MaximumVersion = MTConnectVersion.LookupDeprecated(xmiDocument, umlClass.Id);
                MinimumVersion = MTConnectVersion.LookupNormative(xmiDocument, umlClass.Id);

                // Add SuperClass (ParentType)
                if (umlClass.Generalization != null)
                {
                    ParentName = ModelHelper.GetClassName(xmiDocument, umlClass.Generalization.General);
                    if (ParentName != null && ParentName != "DataItem") ParentName += "DataItem";
                }

                if (umlClass.Properties != null)
                {
                    foreach (var property in umlClass.Properties)
                    {
                        // Default SubType
                        if (property.Name == "subType")
                        {
                            var instanceValue = property.DefaultValue as UmlInstanceValue;
                            if (instanceValue != null)
                            {
                                DefaultSubType = ModelHelper.GetEnumValue(xmiDocument, property.PropertyType, instanceValue.Instance);
                            }
                        }

                        // Result
                        if (property.Name == "result")
                        {
                            // Get Class (TABLE OR DATA_SET)
                            Result = ModelHelper.GetClassName(xmiDocument, property.PropertyType);

                            if (Result == null)
                            {
                                // Get Enum (EVENT)
                                Result = ModelHelper.GetEnumName(xmiDocument, property.PropertyType);
                                Representation = "VALUE";
                            }
                            else
                            {
                                Representation = "TABLE"; // Should probably take into account DATA_SET as well?
                            }
                        }

                        // Units
                        if (property.Name == "units")
                        {
                            var instanceValue = property.DefaultValue as UmlInstanceValue;
                            if (instanceValue != null)
                            {
                                var unitType = ModelHelper.GetEnumName(xmiDocument, property.PropertyType);

                                Units = ModelHelper.GetEnumValue(xmiDocument, property.PropertyType, instanceValue.Instance);
                                //Units = UnitsHelper.Get(Units);
                                if (!string.IsNullOrEmpty(Units)) Units = $"{unitType}.{Units}";
                            }
                        }
                    }
                }

                if (subClasses != null)
                {
                    var subTypes = new List<MTConnectDataItemSubType>();
                    foreach (var subClass in subClasses)
                    {
                        subTypes.Add(new MTConnectDataItemSubType(xmiDocument, Id, subClass));
                    }
                    SubTypes = subTypes;
                }
            }
        }


        public static IEnumerable<MTConnectDataItemType> Parse(XmiDocument xmiDocument, string category, string idPrefix, IEnumerable<UmlClass> umlClasses, UmlEnumeration umlEnumeration)
        {
            var types = new List<MTConnectDataItemType>();

            if (umlClasses != null && umlEnumeration != null)
            {
                foreach (var umlClass in umlClasses)
                {
                    // Filter out SubTypes (ex. Type.Subtype)
                    if (!umlClass.Name.Contains('.'))
                    {
                        var enumItem = umlEnumeration.Items.FirstOrDefault(o => ConvertEnumName(o.Name) == ConvertClassName(umlClass.Name));
                        //var enumItem = umlEnumeration.Items.FirstOrDefault(o => o.Name.ToTitleCase() == umlClass.Name);
                        var subClasses = umlClasses.Where(o => o.Name.StartsWith($"{umlClass.Name}."));

                        types.Add(new MTConnectDataItemType(xmiDocument, category, idPrefix, umlClass, enumItem, subClasses));
                    }
                }
            }

            return types.OrderBy(o => o.Name);
        }


        private static string ConvertClassName(string name)
        {
            if (name != null)
            {
                switch (name)
                {
                    case "AdapterURI": return "AdapterUri"; 
                    case "AmperageAC": return "AmperageAC"; 
                    case "AmperageDC": return "AmperageDC";
                    case "VoltageAC": return "VoltageAC";
                    case "VoltageDC": return "VoltageDC";
                    case "MTConnectVersion": return "MTConnectVersion";
                    case "PH": return "PH";
                    default: return name.ToTitleCase();
                }
            }

            return null;
        }

        private static string ConvertEnumName(string name)
        {
            if (name != null)
            {
                switch (name)
                {
                    case "ADAPTER_URI": return "AdapterUri";
                    case "AMPERAGE_AC": return "AmperageAC";
                    case "AMPERAGE_DC": return "AmperageDC";
                    case "VOLTAGE_AC": return "VoltageAC";
                    case "VOLTAGE_DC": return "VoltageDC";
                    case "PH": return "PH";
                    default: return name.ToTitleCase();
                }
            }

            return null;
        }
    }
}
