using MTConnect.SysML.Xmi;
using MTConnect.SysML.Xmi.UML;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.SysML.Models.Assets
{
    public class MTConnectAssetInformationModel
    {
        public MTConnectAssetModel Asset { get; set; }

        public MTConnectPackageModel ComponentConfigurationParameters { get; set; } = new();

        public MTConnectPackageModel CuttingTools { get; set; } = new();

        public MTConnectPackageModel Files { get; set; } = new();

        public MTConnectPackageModel QIF { get; set; } = new();

        public MTConnectPackageModel RawMaterials { get; set; } = new();


        public MTConnectAssetInformationModel() { }

        public MTConnectAssetInformationModel(XmiDocument xmiDocument)
        {
            Parse(xmiDocument);
        }


        private void Parse(XmiDocument xmiDocument)
        {
            if (xmiDocument != null)
            {
                var umlModel = xmiDocument.Model;

                // Find Information Model in the UML
                var informationModel = umlModel.Packages.FirstOrDefault(o => o.Name == "Asset Information Model");
                if (informationModel != null)
                {
                    var assetClass = informationModel.Classes.FirstOrDefault(o => o.Name == "Asset");
                    Asset = new MTConnectAssetModel(xmiDocument, assetClass);

                    ParseComponentConfigurationParameters(xmiDocument, informationModel);
                    ParseCuttingTools(xmiDocument, informationModel);
                    ParseFiles(xmiDocument, informationModel);
                    ParseQIF(xmiDocument, informationModel);
                    ParseRawMaterials(xmiDocument, informationModel);
                }
            }
        }


        private void ParseComponentConfigurationParameters(XmiDocument xmiDocument, UmlPackage umlPackage)
        {
            var targetPackage = umlPackage.Packages.FirstOrDefault(o => o.Name == "Component Configuration Parameters");
            if (targetPackage != null)
            {
                var umlPackages = ModelHelper.GetPackages(targetPackage);
                var umlClasses = ModelHelper.GetClasses(umlPackages);

                var assetClasses = MTConnectClassModel.Parse(xmiDocument, "Assets.ComponentConfigurationParameters", umlClasses);
                if (assetClasses != null)
                {
                    ComponentConfigurationParameters.Classes.AddRange(assetClasses);
                }
            }
        }

        private void ParseCuttingTools(XmiDocument xmiDocument, UmlPackage umlPackage)
        {
            var cuttingTool = umlPackage.Packages.FirstOrDefault(o => o.Name == "Cutting Tool");
            if (cuttingTool != null)
            {
                // Add Primary Classes
                var packages = new List<UmlPackage>();
                packages.Add(cuttingTool);
                packages.Add(cuttingTool.Packages.FirstOrDefault(o => o.Name == "Cutting Item"));
                packages.Add(cuttingTool.Packages.FirstOrDefault(o => o.Name == "Cutting Tool Life Cycle"));

                var umlClasses = ModelHelper.GetClasses(packages);
                var assetClasses = MTConnectClassModel.Parse(xmiDocument, "Assets.CuttingTools", umlClasses);
                if (assetClasses != null)
                {
                    CuttingTools.Classes.AddRange(assetClasses);
                }


                // Add Measurement Classes
                packages.Clear();
                packages.Add(cuttingTool.Packages.FirstOrDefault(o => o.Name == "Cutting Tool Measurement Subtypes"));

                var cuttingItemPackage = cuttingTool.Packages.FirstOrDefault(o => o.Name == "Cutting Item");
                if (cuttingItemPackage != null)
                {
                    packages.Add(cuttingItemPackage.Packages.FirstOrDefault(o => o.Name == "Cutting Item Measurement Subtypes"));
                }

                umlClasses = ModelHelper.GetClasses(packages);
                var measurementClasses = MTConnectCuttingToolMeasurementModel.Parse(xmiDocument, "CuttingTool", "Assets.CuttingTools.Measurements", umlClasses);
                if (measurementClasses != null)
                {
                    CuttingTools.Classes.AddRange(measurementClasses);
                }


                // Add Enums
                var profile = xmiDocument.Model.Profiles.FirstOrDefault();
                var dataTypes = profile.Packages.FirstOrDefault(o => o.Name == "DataTypes");

                CuttingTools.Enums.Add(new MTConnectEnumModel(xmiDocument, "Assets.CuttingTools", dataTypes?.Enumerations.FirstOrDefault(o => o.Name == "CountDirectionTypeEnum")));
                CuttingTools.Enums.Add(new MTConnectEnumModel(xmiDocument, "Assets.CuttingTools", dataTypes?.Enumerations.FirstOrDefault(o => o.Name == "CutterStatusTypeEnum")));
                CuttingTools.Enums.Add(new MTConnectEnumModel(xmiDocument, "Assets.CuttingTools", dataTypes?.Enumerations.FirstOrDefault(o => o.Name == "ToolLifeEnum")));
                CuttingTools.Enums.Add(new MTConnectEnumModel(xmiDocument, "Assets.CuttingTools", dataTypes?.Enumerations.FirstOrDefault(o => o.Name == "LocationTypeEnum")));
                CuttingTools.Enums.Add(new MTConnectEnumModel(xmiDocument, "Assets.CuttingTools", dataTypes?.Enumerations.FirstOrDefault(o => o.Name == "FormatTypeEnum")));
                CuttingTools.Enums.Add(new MTConnectEnumModel(xmiDocument, "Assets.CuttingTools.Measurements", dataTypes?.Enumerations.FirstOrDefault(o => o.Name == "CodeEnum"), ConvertMeasurementCode));
            }
        }

        private void ParseFiles(XmiDocument xmiDocument, UmlPackage umlPackage)
        {
            var targetPackage = umlPackage.Packages.FirstOrDefault(o => o.Name == "Files");
            if (targetPackage != null)
            {
                var umlPackages = ModelHelper.GetPackages(targetPackage);
                var umlClasses = ModelHelper.GetClasses(umlPackages);
                var assetClasses = MTConnectClassModel.Parse(xmiDocument, "Assets.Files", umlClasses);
                if (assetClasses != null)
                {
                    Files.Classes.AddRange(assetClasses);
                }

                // Add Enums
                var profile = xmiDocument.Model.Profiles.FirstOrDefault();
                var dataTypes = profile.Packages.FirstOrDefault(o => o.Name == "DataTypes");

                Files.Enums.Add(new MTConnectEnumModel(xmiDocument, "Assets.Files", dataTypes?.Enumerations.FirstOrDefault(o => o.Name == "ApplicationCategoryEnum")));
                Files.Enums.Add(new MTConnectEnumModel(xmiDocument, "Assets.Files", dataTypes?.Enumerations.FirstOrDefault(o => o.Name == "ApplicationTypeEnum")));
                Files.Enums.Add(new MTConnectEnumModel(xmiDocument, "Assets.Files", dataTypes?.Enumerations.FirstOrDefault(o => o.Name == "FileStateEnum")));
            }
        }

        private void ParseQIF(XmiDocument xmiDocument, UmlPackage umlPackage)
        {
            var targetPackage = umlPackage.Packages.FirstOrDefault(o => o.Name == "QIF");
            if (targetPackage != null)
            {
                var umlPackages = ModelHelper.GetPackages(targetPackage);
                var umlClasses = ModelHelper.GetClasses(umlPackages);
                var assetClasses = MTConnectClassModel.Parse(xmiDocument, "Assets.QIF", umlClasses);
                if (assetClasses != null)
                {
                    QIF.Classes.AddRange(assetClasses);
                }

                // Add Enums
                var profile = xmiDocument.Model.Profiles.FirstOrDefault();
                var dataTypes = profile.Packages.FirstOrDefault(o => o.Name == "DataTypes");

                QIF.Enums.Add(new MTConnectEnumModel(xmiDocument, "Assets.QIF", dataTypes?.Enumerations.FirstOrDefault(o => o.Name == "QIFDocumentTypeEnum")));
            }
        }

        private void ParseRawMaterials(XmiDocument xmiDocument, UmlPackage umlPackage)
        {
            var targetPackage = umlPackage.Packages.FirstOrDefault(o => o.Name == "Raw Material");
            if (targetPackage != null)
            {
                var umlPackages = ModelHelper.GetPackages(targetPackage);
                var umlClasses = ModelHelper.GetClasses(umlPackages);
                var assetClasses = MTConnectClassModel.Parse(xmiDocument, "Assets.RawMaterials", umlClasses);
                if (assetClasses != null)
                {
                    RawMaterials.Classes.AddRange(assetClasses);
                }

                // Add Enums
                var profile = xmiDocument.Model.Profiles.FirstOrDefault();
                var dataTypes = profile.Packages.FirstOrDefault(o => o.Name == "DataTypes");

                RawMaterials.Enums.Add(new MTConnectEnumModel(xmiDocument, "Assets.RawMaterials", dataTypes?.Enumerations.FirstOrDefault(o => o.Name == "FormEnum")));
            }
        }


        private static string ConvertMeasurementCode(string name)
        {
            if (name == "N/A") return "N_A";

            return name;
        }
    }
}
