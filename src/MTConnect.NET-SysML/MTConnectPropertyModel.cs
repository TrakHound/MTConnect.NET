using MTConnect.SysML.Xmi;
using MTConnect.SysML.Xmi.UML;
using System.Linq;

namespace MTConnect.SysML
{
    public class MTConnectPropertyModel : IMTConnectExportModel
    {
        public string UmlId { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string DataType { get; set; }

        public string DataTypeUmlId { get; set; }

        public bool IsArray { get; set; }


        public MTConnectPropertyModel() { }

        public MTConnectPropertyModel(XmiDocument xmiDocument, string idPrefix, UmlProperty umlProperty) 
        {
            UmlId = umlProperty.Id;

            if (xmiDocument != null && umlProperty != null)
            {
                IsArray = ModelHelper.IsArray(xmiDocument, umlProperty.Id);

                var propertyName = umlProperty.Name;
                if (IsArray && !propertyName.EndsWith("s") && propertyName != "hasToolLife") propertyName += "s";
                if (propertyName.StartsWith("has") && propertyName != "hash") propertyName = propertyName.Substring(3);

                if (propertyName == "xlink:type") propertyName = "xLinkType";

                var name = propertyName.ToTitleCase();

                Id = $"{idPrefix}.{name}";
                Name = name;
                DataType = ParseType(xmiDocument, umlProperty.PropertyType);
                DataTypeUmlId = umlProperty.PropertyType;

                var description = umlProperty.Comments?.FirstOrDefault().Body;
                Description = ModelHelper.ProcessDescription(description);
                if (string.IsNullOrEmpty(Description)) Description = ModelHelper.GetClassDescription(xmiDocument, umlProperty.PropertyType);
            }
        }

        internal static string ParseType(XmiDocument xmiDocument, string typeId)
        {
            if (xmiDocument != null && typeId != null)
            {
                switch (typeId)
                {
                    // string
                    case "_19_0_3_91b028d_1579272360416_763325_681": return "string";

                    // integer
                    case "_19_0_3_91b028d_1579272271512_537408_674": return "int";

                    // boolean
                    case "_19_0_3_91b028d_1579278876899_683310_3821": return "bool";

                    // float
                    case "_19_0_3_91b028d_1579272506322_914606_702": return "double";

                    // DateTime
                    case "_19_0_3_91b028d_1579272233011_597138_670": return "System.DateTime";

                    // Description
                    case "EAID_64352755_7251_46af_846D_937E5A1E3949": return "Description";

                    // ID
                    case "_19_0_3_91b028d_1579272245466_691733_672": return "string";

                    // DataItemTypeEnum
                    case "_19_0_3_45f01b9_1579563576485_587701_22033": return "string";

                    // DataItemSubTypeEnum
                    case "_19_0_3_45f01b9_1579563592155_977172_22064": return "string";

                    default:

                        string dataType = null;

                        var dataClass = ModelHelper.GetClass(xmiDocument, typeId);
                        if (dataClass != null)
                        {
                            if (ModelHelper.IsValueClass(dataClass))
                            {
                                dataType = ModelHelper.GetValueType(xmiDocument, dataClass);
                            }
                            else
                            {
                                dataType = dataClass.Name;
                            }
                        }


                        //var dataType = ModelHelper.GetClassName(xmiDocument, typeId);
                        if (string.IsNullOrEmpty(dataType)) dataType = ModelHelper.GetEnumName(xmiDocument, typeId);
                        if (string.IsNullOrEmpty(dataType)) dataType = "string";
                        return dataType;
                }
            }

            return null;
        }
    }
}
