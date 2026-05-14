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

                var name = ConvertClassName(umlClass.Name);
                //var name = $"{ConvertClassName(umlClass.Name)}DataItem";

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

                var description = umlEnumerationLiteral.Comments?.FirstOrDefault()?.Body;
                Description = ModelHelper.ProcessDescription(description);

                MaximumVersion = MTConnectVersion.LookupDeprecated(xmiDocument, umlClass.Id);
                MinimumVersion = MTConnectVersion.LookupNormative(xmiDocument, umlClass.Id);

                // Add SuperClass (ParentType). DataItem types in MTConnect
                // form a single-inheritance hierarchy — the first
                // generalization is the C# base.
                var dataItemParent = umlClass.Generalizations?.FirstOrDefault();
                if (dataItemParent != null)
                {
                    ParentName = ModelHelper.GetClassName(xmiDocument, dataItemParent.General);
                    //if (ParentName != null && ParentName != "DataItem") ParentName += "DataItem";
                }

                // The MTConnect SysML model declares a DataItem's
                // representation in two complementary places:
                //   * the typing of its `result` property (a DataType
                //     for VALUE; a Class with key/value sub-properties
                //     for the structured forms), and
                //   * the prose marker that the matching EventEnum
                //     literal carries on its description, of the
                //     form `{{term(data set)}}` / `{{term(table)}}`
                //     / `{{term(time series)}}` at the start of the
                //     comment body.
                // Read the prose marker first so DataItems whose
                // structured representation is encoded only in the
                // enum-literal description (ASSET_COUNT being the
                // canonical example) inherit the correct default.
                var representationFromEnum = GetRepresentationFromEnumLiteral(umlEnumerationLiteral);

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
                            // Get Class (structured result). The MTConnect
                            // SysML model encodes the canonical structured
                            // representation in the parent chain of the
                            // result class — `DataSet` for one-dimensional
                            // key/value rows, `Table` for two-dimensional
                            // key/value matrices, `TimeSeries` for sample
                            // sequences. Walk the chain to pick the right
                            // representation; fall back to TABLE so result
                            // classes whose generalization terminates in a
                            // template-binding (e.g. WORK_OFFSETS,
                            // TOOL_OFFSETS) keep their existing default.
                            Result = ModelHelper.GetClassName(xmiDocument, property.PropertyType);

                            if (Result == null)
                            {
                                // Get Enum (EVENT)
                                Result = ModelHelper.GetEnumName(xmiDocument, property.PropertyType);
                                Representation = "VALUE";
                            }
                            else
                            {
                                Representation = ResolveStructuredRepresentation(xmiDocument, property.PropertyType);
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

                // Enum-literal prose wins over the result-class fallback:
                // the SysML XMI uses it as the authoritative marker for the
                // canonical representation of types whose result property
                // points at a primitive DataType.
                if (representationFromEnum != null)
                {
                    Representation = representationFromEnum;
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

        // Walk the generalization chain of a result class to pick the
        // structured representation it encodes. The MTConnect SysML
        // model defines three abstract result-shape parents:
        // `DataSet`, `Table`, and `TimeSeries`; concrete result classes
        // (e.g. AlarmLimitResult, FeatureMeasurementResult) generalize
        // from one of them. A class with no chain match falls back to
        // TABLE so result classes whose generalization terminates in a
        // template-binding (e.g. WORK_OFFSETS, TOOL_OFFSETS) keep their
        // existing default.
        //
        // The walk is bounded by a visited-set rather than a depth cap
        // because a malformed XMI cycle would otherwise loop forever;
        // the visited-set both detects cycles and short-circuits a
        // diamond-inheritance graph that visits the same parent twice.
        private static string ResolveStructuredRepresentation(XmiDocument xmiDocument, string resultClassId)
        {
            var visited = new HashSet<string>();
            var current = ModelHelper.GetClass(xmiDocument, resultClassId);
            while (current != null && visited.Add(current.Id))
            {
                switch (current.Name)
                {
                    case "DataSet": return "DATA_SET";
                    case "Table": return "TABLE";
                    case "TimeSeries": return "TIME_SERIES";
                }

                var parentId = current.Generalizations?.FirstOrDefault()?.General;
                if (string.IsNullOrEmpty(parentId)) break;
                current = ModelHelper.GetClass(xmiDocument, parentId);
            }

            return "TABLE";
        }

        // The MTConnect SysML model embeds the canonical representation
        // of an EventEnum literal in its description. The marker is one
        // of `{{term(data set)}}`, `{{term(table)}}`, or
        // `{{term(time series)}}` and appears at the start of the
        // comment body. When present, it overrides the result-property
        // typing fallback so DataItems whose `result` references a
        // primitive DataType (e.g. ASSET_COUNT pointing at `integer`)
        // still inherit the structured representation the spec mandates.
        private static string GetRepresentationFromEnumLiteral(UmlEnumerationLiteral umlEnumerationLiteral)
        {
            var body = umlEnumerationLiteral?.Comments?.FirstOrDefault()?.Body;
            if (string.IsNullOrEmpty(body)) return null;

            var trimmed = body.TrimStart();
            if (trimmed.StartsWith("{{term(data set)}}")) return "DATA_SET";
            if (trimmed.StartsWith("{{term(table)}}")) return "TABLE";
            if (trimmed.StartsWith("{{term(time series)}}")) return "TIME_SERIES";

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
                    case "MTCONNECT_VERSION": return "MTConnectVersion";
                    case "PH": return "PH";
                    default: return name.ToTitleCase();
                }
            }

            return null;
        }
    }
}
