using MTConnect.SysML.Xmi;
using MTConnect.SysML.Xmi.UML;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MTConnect.SysML
{
    internal static class ModelHelper
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
                    case "FilterEnum": name = "FilterTypeEnum"; break;
                    case "NativeUnitEnum": name = "NativeUnitsEnum"; break;
                    case "RepresentationEnum": name = "DataItemRepresentationEnum"; break;
                    case "StatisticEnum": name = "DataItemStatisticEnum"; break;
                    case "UnitEnum": name = "UnitsEnum"; break;
                }

                //var suffix = "Enum";
                //if (name.EndsWith(suffix)) name = name.Substring(0, name.Length - suffix.Length);
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
                words[0] = words[0].UppercaseFirstCharacter();
                return string.Join(' ', words);
            }

            return null;
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

                case "_19_0_3_45f01b9_1581433281431_574073_286": return true; // Devices.DataItems.Definition.CellDefinitions
                case "_19_0_3_45f01b9_1582939789522_356798_4412": return true; // Devices.DataItems.Definition.EntryDefinitions

                case "EAID_CB3DBE83_DB10_4aa0_9685_72CC1BEA5285": return true; // Assets.CuttingTool.Manufacturers

                case "EAID_E485DD9D_8788_4c57_B422_E3374F4215DC": return true; // Assets.CuttingToolArchetype.Manufacturers

                case "EAID_F7C32A8C_8166_4c26_839E_F946E18DB022": return true; // Assets.CuttingItem.Indices
                case "EAID_9E5855C8_F90D_4ddf_A2FC_4610634008B4": return true; // Assets.CuttingItem.Manufacturers
                case "_19_0_3_91b028d_1582658982276_248635_527": return true; // Assets.CuttingItem.Measurements

                case "EAID_dst8C9154_3651_4af3_9311_2AA3D0EF7282": return true; // Assets.CuttingToolLifeCycle.ToolLife
                case "EAID_dstC908A3_8442_4a95_AB3E_68DFB6274CDD": return true; // Assets.CuttingToolLifeCycle.CutterStatus
                case "EAID_dstAB92D5_E33E_4e6e_92EB_1FFFBF29ED9F": return true; // Assets.CuttingToolLifeCycle.CuttingItem
                case "EAID_dst6C3AA0_3DE7_43bf_B6D6_22C9350D4FE2": return true; // Assets.CuttingToolLifeCycle.Measurement

                case "_19_0_3_68e0225_1622116618960_627070_1641": return true; // Assets.RawMaterial.InitialDimension
                case "_19_0_3_68e0225_1622116618964_666287_1642": return true; // Assets.RawMaterial.CurrentDimension

                case "_19_0_3_68e0225_1678197264958_675939_17962": return true; // Assets.ComponentConfigurationParameters.ParameterSet
                case "_19_0_3_68e0225_1678197386869_402580_18053": return true; // Assets.ParameterSet.Parameters
            }

            return false;
        }


        public static bool IsValueClass(UmlClass umlClass)
        {
            if (umlClass != null && umlClass.Generalization == null)
            {
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
                        return MTConnectPropertyModel.ParseType(xmiDocument, valueProperty.PropertyType);
                    }
                }
            }

            return "string";
        }
    }
}
