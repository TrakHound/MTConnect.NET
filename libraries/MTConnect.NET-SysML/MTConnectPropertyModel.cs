﻿using MTConnect.SysML.Xmi;
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

        public bool IsOptional { get; set; }

        public bool IsArray { get; set; }


        public MTConnectPropertyModel() { }

        public MTConnectPropertyModel(XmiDocument xmiDocument, string idPrefix, UmlProperty umlProperty) 
        {
            UmlId = umlProperty.Id;

            if (xmiDocument != null && umlProperty != null)
            {
                IsArray = ModelHelper.IsArray(xmiDocument, umlProperty.Id);
                IsOptional = ModelHelper.IsOptional(xmiDocument, umlProperty.Id);

                var propertyName = umlProperty.Name;
                if (propertyName.StartsWith("has") && propertyName != "hash") propertyName = propertyName.Substring(3);

                if (propertyName == "xlink:type") propertyName = "xLinkType";

                var name = propertyName.ToTitleCase();
                if (IsArray) name = ModelHelper.ConvertArrayName(name);

                Id = $"{idPrefix}.{name}";
                Name = name;
                DataType = ParseType(xmiDocument, umlProperty.Id, umlProperty.PropertyType);
                DataTypeUmlId = umlProperty.PropertyType;

                var description = umlProperty.Comments?.FirstOrDefault().Body;
                Description = ModelHelper.ProcessDescription(description);
                if (string.IsNullOrEmpty(Description)) Description = ModelHelper.GetClassDescription(xmiDocument, umlProperty.PropertyType);
            }
        }

        internal static string ParseType(XmiDocument xmiDocument, string propertyId, string typeId)
        {
            if (xmiDocument != null && propertyId != null && typeId != null)
            {
                switch (propertyId)
                {
                    // CoordinateSystem.Origin
                    case "_19_0_3_45f01b9_1579107788324_454462_163661": return "UNIT_VECTOR_3D";

                    // SolidModel.Scale
                    case "_19_0_3_45f01b9_1587596366617_434199_806": return "UNIT_VECTOR_3D";

                    // Motion.Axis
                    case "_19_0_3_91b028d_1579531167395_857364_8288": return "UNIT_VECTOR_3D";

                    // Motion.Origin
                    case "_19_0_3_91b028d_1579531211501_359270_8311": return "UNIT_VECTOR_3D";

                    // Translation.Rotation
                    case "_19_0_3_45f01b9_1583182442343_989150_4833": return "DEGREE_3D";

                    // Translation.Translation
                    case "_19_0_3_45f01b9_1579106868983_196924_163307": return "UNIT_VECTOR_3D";

                    // RawMaterials.RawMateral.CurrentVolume
                    case "_19_0_3_68e0225_1618831247227_54016_392": return "double";

                    // RawMaterials.RawMateral.CurrentDimension
                    case "_19_0_3_68e0225_1622116618964_666287_1642": return "MILLIMETER_3D";

                    // RawMaterials.RawMateral.InitialVolume
                    case "_19_0_3_68e0225_1618831175692_489264_387": return "double";

                    // RawMaterials.RawMateral.InitialDimension
                    case "_19_0_3_68e0225_1622116618960_627070_1641": return "MILLIMETER_3D";


                    // CuttingTools.ProcessFeedRate.Value (incorrect in MTConnect Model 2.2)
                    case "_19_0_3_68e0225_1636117526335_679126_67": return "double";
                }


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

                    // double
                    case "_19_0_3_68e0225_1678197512818_76309_18111": return "double";

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
