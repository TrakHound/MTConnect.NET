using MTConnect.SysML.Xmi;
using MTConnect.SysML.Xmi.UML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MTConnect.SysML
{
    public static class ModelHelper
    {
        private static Dictionary<string, UmlPackage> _packages;
        private static Dictionary<string, UmlClass> _classes;
        private static Dictionary<string, UmlEnumeration> _enumerations;


        public static UmlClass GetClass(XmiDocument xmiDocument, string id)
        {
            if (xmiDocument != null && !string.IsNullOrEmpty(id))
            {
                if (_packages == null) InitializePackages(xmiDocument);
                if (_classes == null) InitializeClasses();

                return _classes.GetValueOrDefault(id);
            }

            return null;
        }

        public static string GetClassName(XmiDocument xmiDocument, string id)
        {
            if (xmiDocument != null && !string.IsNullOrEmpty(id))
            {
                if (_packages == null) InitializePackages(xmiDocument);
                if (_classes == null) InitializeClasses();

                var match = _classes.GetValueOrDefault(id);
                if (match != null) return match.Name;
            }

            return null;
        }

        public static string GetClassDescription(XmiDocument xmiDocument, string id)
        {
            if (xmiDocument != null && !string.IsNullOrEmpty(id))
            {
                if (_packages == null) InitializePackages(xmiDocument);
                if (_classes == null) InitializeClasses();

                var match = _classes.GetValueOrDefault(id);
                if (match != null)
                {
                    var description = match.Comments?.FirstOrDefault().Body;
                    return ProcessDescription(description);
                }
            }

            return null;
        }

        public static UmlEnumeration GetEnum(XmiDocument xmiDocument, string typeId)
        {
            if (xmiDocument != null && !string.IsNullOrEmpty(typeId))
            {
                if (_packages == null) InitializePackages(xmiDocument);
                if (_enumerations == null) InitializeEnumerations();

                return _enumerations.GetValueOrDefault(typeId);
            }

            return null;
        }

        public static string GetEnumName(XmiDocument xmiDocument, string typeId)
        {
            if (xmiDocument != null && !string.IsNullOrEmpty(typeId))
            {
                if (_packages == null) InitializePackages(xmiDocument);
                if (_enumerations == null) InitializeEnumerations();

                var enumeration = _enumerations.GetValueOrDefault(typeId);
                if (enumeration != null)
                {
                    return ConvertEnumName(enumeration.Name);
                }
            }

            return null;
        }

        public static string GetEnumValue(XmiDocument xmiDocument, string typeId, string valueId)
        {
            if (xmiDocument != null && !string.IsNullOrEmpty(typeId) && !string.IsNullOrEmpty(valueId))
            {
                if (_packages == null) InitializePackages(xmiDocument);
                if (_enumerations == null) InitializeEnumerations();

                var enumeration = _enumerations.GetValueOrDefault(typeId);
                if (enumeration != null)
                {
                    var item = enumeration.Items.FirstOrDefault(o => o.Id == valueId);
                    if (item != null)
                    {
                        return item.Name;
                    }
                }
            }

            return null;
        }

        public static string ConvertEnumName(string name)
        {
            if (name != null)
            {
                switch (name)
                {
                    case "CategoryEnum": name = "DataItemCategoryEnum"; break;
                    case "CodeEnum": name = "MeasurementCodeEnum"; break;
                    case "CoordinateSystemEnum": name = "DataItemCoordinateSystemEnum"; break;
                    case "FilterEnum": name = "DataItemFilterTypeEnum"; break;
                    case "NativeUnitEnum": name = "NativeUnitsEnum"; break;
                    case "RepresentationEnum": name = "DataItemRepresentationEnum"; break;
                    case "ResetTriggerEnum": name = "DataItemResetTriggerEnum"; break;
                    case "StatisticEnum": name = "DataItemStatisticEnum"; break;
                    case "ToolLifeEnum": name = "ToolLifeTypeEnum"; break;
                    case "UnitEnum": name = "UnitsEnum"; break;
                }
            }

            return name;
        }

        public static string RemoveEnumSuffix(string name)
        {
            if (name != null)
            {
                var suffix = "Enum";
                if (name.EndsWith(suffix)) name = name.Substring(0, name.Length - suffix.Length);
            }

            return name;
        }

        private static IEnumerable<UmlPackage> GetAllPackages(UmlModel umlModel)
        {
            var packages = new List<UmlPackage>();

            foreach (var package in umlModel.Packages)
            {
                packages.AddRange(GetPackages(package));
            }

            foreach (var profile in umlModel.Profiles)
            {
                packages.AddRange(GetPackages(profile));
            }

            return packages;
        }

        public static IEnumerable<UmlPackage> GetPackages(UmlPackage package)
        {
            var packages = new List<UmlPackage>();
            packages.Add(package);

            if (package.Packages != null)
            {
                foreach (var childPackage in package.Packages)
                {
                    packages.AddRange(GetPackages(childPackage));
                }
            }

            return packages;
        }

        private static IEnumerable<UmlPackage> GetPackages(UmlProfile profile)
        {
            var packages = new List<UmlPackage>();

            if (profile.Packages != null)
            {
                foreach (var childPackage in profile.Packages)
                {
                    packages.AddRange(GetPackages(childPackage));
                }
            }

            return packages;
        }

        public static IEnumerable<UmlClass> GetClasses(IEnumerable<UmlPackage> packages)
        {
            var classes = new List<UmlClass>();

            foreach (var package in packages)
            {
                if (package.Classes != null)
                {
                    foreach (var packageClass in package.Classes)
                    {
                        classes.Add(packageClass);
                    }
                }
            }

            return classes;
        }

        private static IEnumerable<UmlEnumeration> GetEnumerations(IEnumerable<UmlPackage> packages)
        {
            var enumerations = new List<UmlEnumeration>();

            foreach (var package in packages)
            {
                if (package.Enumerations != null)
                {
                    foreach (var packageEnumeration in package.Enumerations)
                    {
                        enumerations.Add(packageEnumeration);
                    }
                }
            }

            return enumerations;
        }


        private static void InitializePackages(XmiDocument xmiDocument)
        {
            var umlModel = xmiDocument.Model;
            var packages = GetAllPackages(umlModel);
            if (packages != null)
            {
                var dPackages = new Dictionary<string, UmlPackage>();
                foreach (var package in packages)
                {
                    dPackages.Remove(package.Id);
                    dPackages.Add(package.Id, package);
                }
                _packages = dPackages;
            }
        }

        private static void InitializeClasses()
        {
            var classes = GetClasses(_packages.Values);
            if (classes != null)
            {
                var dClasses = new Dictionary<string, UmlClass>();

                foreach (var packageClass in classes)
                {
                    dClasses.Remove(packageClass.Id);
                    dClasses.Add(packageClass.Id, packageClass);
                }

                _classes = dClasses;
            }
        }

        private static void InitializeEnumerations()
        {
            var enumerations = GetEnumerations(_packages.Values);
            if (enumerations != null)
            {
                var dEnumerations = new Dictionary<string, UmlEnumeration>();

                foreach (var packageEnumeration in enumerations)
                {
                    dEnumerations.Remove(packageEnumeration.Id);
                    dEnumerations.Add(packageEnumeration.Id, packageEnumeration);
                }

                _enumerations = dEnumerations;
            }
        }


        public static string ProcessDescription(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                var regex = new Regex("\\{\\{.*?\\((.*?)\\)\\}\\}");
                var result = text;

                var matches = regex.Matches(text);
                if (matches != null)
                {
                    foreach (Match match in matches)
                    {
                        var original = match.Groups[0].Value;
                        var replace = match.Groups[1].Value;

                        result = result.Replace(original, replace);
                    }
                }

                result = result.Replace("\n", "");
                result = result.Replace("\r", "");
                result = result.Replace("\"", "'");
                result = result.Replace("Types::", "");
                result = UppercaseFirstWord(result);


                //result = result.Replace(".", ". ");
                //result = result.UppercaseFirstCharacter();

                
                result = result.Replace("Mtconnect", "MTConnect");
                result = result.Replace("mtconnect", "MTConnect");
                result = result.Trim();

                return result;
            }

            return null;
        }

        private static string UppercaseFirstWord(string text)
        {
            if (text != null && text.Length > 0)
            {
                var words = text.Split(' ');
                words[0] = UppercaseFirstCharacter(words[0]);

                return string.Join(' ', words);
            }

            return null;
        }

        private static string UppercaseFirstCharacter(string s)
        {
            if (s == null) return null;

            if (s.Length > 1)
            {
                var l = s.ToCharArray();
                var a = new char[l.Length];

                a[0] = char.ToUpper(l[0]);
                Array.Copy(l, 1, a, 1, a.Length - 1);

                return new string(a);
            }

            return s.ToUpper();
        }


        public static bool IsArray(XmiDocument xmiDocument, string id)
        {
            switch (id)
            {
                case "_19_0_3_91b028d_1579274935610_708920_3095": return true; // Devices.Component.Components
                case "_19_0_3_91b028d_1579274803419_180043_3064": return true; // Devices.Component.Compositions
                case "_19_0_3_45f01b9_1581211888318_232581_149": return true; // Devices.Component.References

                case "_19_0_3_68e0225_1633431910074_887850_97": return true; // Devices.Configuration.Relationships
                case "_19_0_3_68e0225_1633431923171_707595_113": return true;  // Devices.Configuration.CoordinateSystems
                case "_19_0_3_68e0225_1633431989416_861348_140": return true; // Devices.Configuration.Specifications
                case "_19_0_3_68e0225_1677585034568_640359_707": return true; // Devices.Configuration.ImageFiles

                case "EAID_src1FD414_08E5_4c06_8E6A_D0FBEE71B296": return true; // Devices.Configruation.SensorConfiguration.Channels

                case "_19_0_3_91b028d_1579280419002_422759_4126": return true; // Devices.DataItem.Filters
                case "_19_0_3_68e0225_1607601081190_91136_31": return true; // Devices.DataItem.Relationships
                case "EAID_303DE47E_9DA0_45be_9630_C12FCD3FCB23": return true; // Devices.Contstraints.Value

                case "_19_0_3_45f01b9_1581433281431_574073_286": return true; // Devices.DataItems.Definition.CellDefinitions
                case "_19_0_3_45f01b9_1582939789522_356798_4412": return true; // Devices.DataItems.Definition.EntryDefinitions
                case "_19_0_3_45f01b9_1582940123488_979807_4523": return true; // Devices.DataItems.EntryDefinition.CellDefinitions

                case "EAID_CB3DBE83_DB10_4aa0_9685_72CC1BEA5285": return true; // Assets.CuttingTool.Manufacturers

                case "EAID_E485DD9D_8788_4c57_B422_E3374F4215DC": return true; // Assets.CuttingToolArchetype.Manufacturers

                case "EAID_dst225492_D39D_4863_B945_37824D539BEE": return true; // Assets.CuttingItem.CutterStatus
                //case "EAID_F7C32A8C_8166_4c26_839E_F946E18DB022": return true; // Assets.CuttingItem.Indices
                case "EAID_9E5855C8_F90D_4ddf_A2FC_4610634008B4": return true; // Assets.CuttingItem.Manufacturers
                case "_19_0_3_91b028d_1582658982276_248635_527": return true; // Assets.CuttingItem.Measurements
                case "EAID_dst4843A3_8ECC_42a0_9DC5_9F8247F1E3C7": return true; // Assets.CuttingItem.ItemLife

                case "EAID_dst8C9154_3651_4af3_9311_2AA3D0EF7282": return true; // Assets.CuttingToolLifeCycle.ToolLife
                case "EAID_dstC908A3_8442_4a95_AB3E_68DFB6274CDD": return true; // Assets.CuttingToolLifeCycle.CutterStatus
                case "EAID_dstAB92D5_E33E_4e6e_92EB_1FFFBF29ED9F": return true; // Assets.CuttingToolLifeCycle.CuttingItem
                case "EAID_dst6C3AA0_3DE7_43bf_B6D6_22C9350D4FE2": return true; // Assets.CuttingToolLifeCycle.Measurement

                case "_19_0_3_68e0225_1605276232723_226459_243": return true; // Assets.Files.AbstractFile.FileComments
                case "_19_0_3_45f01b9_1589825726302_711121_874": return true; // Assets.Files.AbstractFile.FileProperties

                case "_19_0_3_68e0225_1605277201359_44575_523": return true; // Assets.Files.File.Destinations

                //case "_19_0_3_68e0225_1622116618960_627070_1641": return true; // Assets.RawMaterial.InitialDimension
                //case "_19_0_3_68e0225_1622116618964_666287_1642": return true; // Assets.RawMaterial.CurrentDimension

                case "_19_0_3_68e0225_1678197264958_675939_17962": return true; // Assets.ComponentConfigurationParameters.ParameterSet
                case "_19_0_3_68e0225_1678197386869_402580_18053": return true; // Assets.ParameterSet.Parameters
            }

            return false;
        }

        public static string ConvertArrayName(string name)
        {
            if (name != null)
            {
                switch (name)
                {
                    case "ToolLife": return "ToolLife";
                    case "ItemLife": return "ItemLife";
                    case "FileProperty": return "FileProperties";
                    default:
                        if (!name.EndsWith("s")) return name += "s";
                        break;
                
                }
            }

            return name;
        }

        public static bool IsOptional(XmiDocument xmiDocument, string id)
        {
            switch (id)
            {
                case "EAID_C7968EFA_A55E_4ccf_B7CA_CFE13D86C116": return true; // Devices.DataItem.Maximum
                case "EAID_72FA9526_7E5D_4084_A7F0_39FD402026E6": return true; // Devices.DataItem.Minimum
                case "EAID_7B83A56F_E21C_4aa3_9851_413935CE01A2": return true; // Devices.DataItem.Nominal

                case "_19_0_3_91b028d_1579280424778_62185_4160": return true; // Devices.DataItem.ResetTrigger
                case "EAID_FE16FE90_F8E4_4832_8D49_C35DD4857F52": return true; // Devices.DataItem.SignificantDigits
                case "EAID_DE2DBB93_2F2D_4020_981C_C9302713E54C": return true; // Devices.DataItem.Statistic

                case "_19_0_3_68e0225_1607601584366_130474_71": return true; // Devices.Configruations.ConfigurationRelationship.Criticality
                case "EAID_6CAB5838_437F_4458_BB16_CB0E6FC7F3D3": return true; // Devices.Configruations.DeviceRelationship.Role

                case "_19_0_3_68e0225_1605644630392_83676_2528": return true; // Devices.Configurations.SpecificationLimits.LowerLimit
                case "_19_0_3_68e0225_1605644626222_931495_2512": return true; // Devices.Configurations.SpecificationLimits.Nominal
                case "_19_0_3_68e0225_1605644623289_677234_2496": return true; // Devices.Configurations.SpecificationLimits.UpperLimit

                case "_19_0_3_68e0225_1605644652700_647829_2576": return true; // Devices.Configurations.AlarmLimits.LowerLimit
                case "_19_0_3_68e0225_1605644655989_576567_2592": return true; // Devices.Configurations.AlarmLimits.LowerWarning
                case "_19_0_3_68e0225_1605644643483_185781_2544": return true; // Devices.Configurations.AlarmLimits.UpperLimit
                case "_19_0_3_68e0225_1605644647420_931185_2560": return true; // Devices.Configurations.AlarmLimits.UpperWarning

                case "_19_0_3_68e0225_1636568068616_675126_84": return true; // Devices.Configurations.Specification.Maximum
                case "_19_0_3_68e0225_1636568073147_54408_100": return true; // Devices.Configurations.Specification.UpperLimit
                case "_19_0_3_68e0225_1636568078240_370406_116": return true; // Devices.Configurations.Specification.LowerWarning
                case "_19_0_3_68e0225_1636568080906_86201_132": return true; // Devices.Configurations.Specification.LowerLimit
                case "_19_0_3_68e0225_1636568084399_748206_148": return true; // Devices.Configurations.Specification.UpperWarning
                case "_19_0_3_68e0225_1636568086926_172243_164": return true; // Devices.Configurations.Specification.Nominal
                case "_19_0_3_68e0225_1636568090268_102258_180": return true; // Devices.Configurations.Specification.Minimum

                case "_19_0_3_68e0225_1605644563152_491205_2336": return true; // Devices.Configurations.ControlLimits.UpperLimit
                case "_19_0_3_68e0225_1605644567845_98299_2368": return true; // Devices.Configurations.ControlLimits.LowerWarning
                case "_19_0_3_68e0225_1605644572644_365697_2400": return true; // Devices.Configurations.ControlLimits.LowerLimit
                case "_19_0_3_68e0225_1605644565736_276152_2352": return true; // Devices.Configurations.ControlLimits.UpperWarning
                case "_19_0_3_68e0225_1605644570052_987268_2384": return true; // Devices.Configurations.ControlLimits.Nominal

                case "EAID_3A147FC7_7743_4113_B7C5_39898A2FCCDC": return true; // Devices.Configurations.SensorConfiguration.CalibrationDate
                case "EAID_CF5F303B_1708_4731_8BAA_004C65E8A073": return true; // Devices.Configurations.SensorConfiguration.NextCalibrationDate

                case "EAID_5E07F763_7B89_42ba_9C06_01DD7CD5E39B": return true; // Devices.Configurations.Channel.CalibrationDate
                case "EAID_CBE261CF_F527_472a_809F_30CB0C4887C3": return true; // Devices.Configurations.Channel.NextCalibrationDate

                case "EAID_816C2A06_ABDB_4bc5_B6B9_81DB30748E81": return true; // Assets.CuttingTools.ToolLife.Initial
                case "EAID_F82F6CC7_3926_4b9a_A2EC_B4E33841EA55": return true; // Assets.CuttingTools.ToolLife.Limit
                case "EAID_85A88677_0C19_4f98_BC72_D9C12AADEC96": return true; // Assets.CuttingTools.ToolLife.Warning

                case "EAID_3C9431B8_07D3_447e_8AB2_57092629A506": return true; // Assets.CuttingTools.ItemLife.Initial
                case "EAID_10171B17_FEA7_4320_89AE_C0EF01C53A08": return true; // Assets.CuttingTools.ItemLife.Limit
                case "EAID_96660E74_7536_4ed9_852E_F058E7959BE7": return true; // Assets.CuttingTools.ItemLife.Warning

                case "EAID_03F2782A_F815_48c6_9768_20270E8E7815": return true; // Assets.CuttingTools.Location.NegativeOverlap
                case "EAID_1714E6CA_F95C_45c2_8A79_7A5DD3A758B0": return true; // Assets.CuttingTools.Location.PositiveOverlap

                case "EAID_7879B467_C3DF_46ed_8EFF_525BAEBF408E": return true; // Assets.CuttingTools.Measurement.Maximum
                case "EAID_786BC04E_2E91_4c0c_98A0_E5F0FE18D3F8": return true; // Assets.CuttingTools.Measurement.Minimum
                case "EAID_F49DCFE5_6CDE_40bc_AD09_CC685107A9BA": return true; // Assets.CuttingTools.Measurement.Nominal
                case "EAID_E8029A72_0F82_4977_AD41_978D749E3845": return true; // Assets.CuttingTools.Measurement.SignificantDigits
                case "_19_0_3_68e0225_1636118507748_705715_75": return true; // Assets.CuttingTools.Measurement.Value

                case "EAID_9C794E2F_70E7_48ef_B2C6_CCF96033981D": return true; // Assets.CuttingTools.ProcessFeedRate.Maximum
                case "EAID_B7DA2543_0482_4a2d_B5BD_2DD529A63DC4": return true; // Assets.CuttingTools.ProcessFeedRate.Minimum
                case "EAID_11C5DEFD_AD02_485a_A491_459FA25B2A32": return true; // Assets.CuttingTools.ProcessFeedRate.Nominal
                case "_19_0_3_68e0225_1636117526335_679126_67": return true; // Assets.CuttingTools.ProcessFeedRate.Value

                case "EAID_4F5F7CB2_ED9F_46c2_B4BB_EE9D41608A0F": return true; // Assets.CuttingTools.ProcessSpindleSpeed.Maximum
                case "EAID_A8DED60C_FFB4_43bf_BC80_AFBEF1EBDFAB": return true; // Assets.CuttingTools.ProcessSpindleSpeed.Minimum
                case "EAID_E5A5CE64_0C68_4a21_9B39_8C5167039B06": return true; // Assets.CuttingTools.ProcessSpindleSpeed.Nominal
                case "_19_0_3_68e0225_1636117179847_339275_62": return true; // Assets.CuttingTools.ProcessSpindleSpeed.Value

                case "EAID_320E255E_EDA1_49b8_928D_8A87BE90CF06": return true; // Assets.CuttingTools.ReconditionCount.MaximumCount
                case "_19_0_3_68e0225_1636115275525_86177_53": return true; // Assets.CuttingTools.ReconditionCount.Value

                case "_19_0_3_68e0225_1605276845966_494050_370": return true; // Assets.Files.File.ModificationTime

                //case "_19_0_3_68e0225_1622116618964_666287_1642": return true; // Assets.RawMaterials.RawMaterial.CurrentDimension
                case "_19_0_3_68e0225_1622116620205_160609_1656": return true; // Assets.RawMaterials.RawMaterial.CurrentQuantity
                case "_19_0_3_68e0225_1618831247227_54016_392": return true; // Assets.RawMaterials.RawMaterial.CurrentVolume
                case "_19_0_3_68e0225_1618831102942_637392_372": return true; // Assets.RawMaterials.RawMaterial.FirstUseDate
                case "_19_0_3_68e0225_1618831263908_747143_397": return true; // Assets.RawMaterials.RawMaterial.HasMaterial
                //case "_19_0_3_68e0225_1622116618960_627070_1641": return true; // Assets.RawMaterials.RawMaterial.InitialDimension
                case "_19_0_3_68e0225_1622116620204_269519_1655": return true; // Assets.RawMaterials.RawMaterial.InitialQuantity
                case "_19_0_3_68e0225_1618831175692_489264_387": return true; // Assets.RawMaterials.RawMaterial.InitialVolume
                case "_19_0_3_68e0225_1618831134662_505390_377": return true; // Assets.RawMaterials.RawMaterial.LastUseDate
                case "_19_0_3_68e0225_1618831023683_973378_367": return true; // Assets.RawMaterials.RawMaterial.ManufacturingDate

                case "_19_0_3_68e0225_1618831426887_198644_412": return true; // Assets.RawMaterials.Material.ManufacturingDate

                case "_19_0_3_68e0225_1678197448351_159297_18088": return true; // Assets.ComponentConfigurationParameters.Parameter.Maximum
                case "_19_0_3_68e0225_1678197439753_242271_18083": return true; // Assets.ComponentConfigurationParameters.Parameter.Minimum
                case "_19_0_3_68e0225_1678197459368_734960_18093": return true; // Assets.ComponentConfigurationParameters.Parameter.Nominal
            }

            return false;
        }


        public static bool IsValueClass(UmlClass umlClass)
        {
            if (umlClass != null && umlClass.Generalization == null)
            {
                if (umlClass.Name == "Destination") return false;


                var umlProperties = umlClass.Properties?.Where(o => !o.Name.StartsWith("made") && !o.Name.StartsWith("is") && !o.Name.StartsWith("observes"));
                if (umlProperties != null && umlProperties.Count() == 1)
                {
                    var valueProperty = umlProperties.FirstOrDefault();
                    if (valueProperty != null)
                    {
                        System.Console.WriteLine(valueProperty.Name);

                        return true;
                    }

                    //var valueProperty = umlProperties.FirstOrDefault(o => o.Name == "Value");
                    //if (valueProperty != null)
                    //{
                    //    return true;
                    //}
                }
            }

            return false;
        }

        public static string GetValueType(XmiDocument xmiDocument, UmlClass umlClass)
        {
            if (umlClass != null)
            {
                var umlProperties = umlClass.Properties?.Where(o => !o.Name.StartsWith("made") && !o.Name.StartsWith("is") && !o.Name.StartsWith("observes"));
                if (umlProperties != null && umlProperties.Count() == 1)
                {
                    var valueProperty = umlProperties.FirstOrDefault();
                    //var valueProperty = umlProperties.FirstOrDefault(o => o.Name == "Value");
                    if (valueProperty != null)
                    {
                        return MTConnectPropertyModel.ParseType(xmiDocument, valueProperty.Id, valueProperty.PropertyType);
                    }
                }
            }

            return "string";
        }
    }
}
