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

        public bool IsOptional { get; set; }

        public bool IsArray { get; set; }

        /// <summary>
        /// True when the property's <see cref="Name"/> hides an inherited member on
        /// the generated <em>class</em> declaration. Model.scriban and the
        /// DataSetResults template emit a <c>new</c> modifier on the property when
        /// this is set, suppressing CS0108 ("hides inherited member; use the new
        /// keyword if hiding was intended"). Populated by the per-renderer
        /// inheritance pass — the SysML-declared parent chain plus hand-stitched
        /// seeds for class-side inheritance links the SysML model does not express
        /// (the hand-written <c>Observation</c> base of every DataSetResult).
        /// </summary>
        public bool IsInherited { get; set; }

        /// <summary>
        /// True when the property's <see cref="Name"/> hides an inherited member on
        /// the generated <em>interface</em> declaration. Interface.scriban emits a
        /// <c>new</c> modifier on the property when this is set, suppressing the
        /// same CS0108. Separate from <see cref="IsInherited"/> because the
        /// inheritance picture can diverge between the class and interface sides —
        /// e.g. <c>IComposition</c>'s hand-written partial extends <c>IContainer</c>
        /// (interface hides <c>Type</c>) but <c>Composition</c>'s hand-written
        /// partial does not extend <c>Container</c> as a class base (the class does
        /// not hide). The renderer's inheritance walk seeds both flags from the
        /// SysML chain and adds interface-only seeds where the hand-written
        /// interface partial extends a base the class does not.
        /// </summary>
        public bool IsInheritedInInterface { get; set; }


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
