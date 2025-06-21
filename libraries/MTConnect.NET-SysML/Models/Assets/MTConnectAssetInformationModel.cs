using MTConnect.SysML.Xmi;
using MTConnect.SysML.Xmi.UML;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.SysML.Models.Assets
{
    public class MTConnectAssetInformationModel
    {
        public MTConnectAssetModel Asset { get; set; }

        public MTConnectPhysicalAssetModel PhysicalAsset { get; set; }

        public MTConnectPackageModel ComponentConfigurationParameters { get; set; } = new();

        public MTConnectPackageModel CuttingTools { get; set; } = new();

        public MTConnectPackageModel Files { get; set; } = new();

        public MTConnectPackageModel Fixtures { get; set; } = new();

        public MTConnectPackageModel Pallets { get; set; } = new();

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
                    var physicalAssetClass = informationModel.Classes.FirstOrDefault(o => o.Name == "PhysicalAsset");

                    Asset = new MTConnectAssetModel(xmiDocument, assetClass);
                    PhysicalAsset = new MTConnectPhysicalAssetModel(xmiDocument, physicalAssetClass);

                    ParseComponentConfigurationParameters(xmiDocument, informationModel);
                    ParseCuttingTools(xmiDocument, informationModel);
                    ParseFiles(xmiDocument, informationModel);
                    ParseFixtures(xmiDocument, informationModel);
                    ParsePallets(xmiDocument, informationModel);
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
                    //foreach (var assetClass in assetClasses)
                    //{
                    //    if (assetClass.Id != "")
                    //    {

                    //    }
                    //}

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
                var measurementClasses = MTConnectMeasurementModel.Parse(xmiDocument, "CuttingTool", "Assets.CuttingTools.Measurements", umlClasses);
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

        private void ParseFixtures(XmiDocument xmiDocument, UmlPackage umlPackage)
        {
            var targetPackage = umlPackage.Packages.FirstOrDefault(o => o.Name == "Fixture");
            if (targetPackage != null)
            {
                var umlPackages = ModelHelper.GetPackages(targetPackage);
                var umlClasses = ModelHelper.GetClasses(umlPackages);
                var assetClasses = MTConnectClassModel.Parse(xmiDocument, "Assets.Fixture", umlClasses);
                if (assetClasses != null)
                {
                    Fixtures.Classes.AddRange(assetClasses);
                }
            }
        }

        private void ParsePallets(XmiDocument xmiDocument, UmlPackage umlPackage)
        {
            var targetPackage = umlPackage.Packages.FirstOrDefault(o => o.Name == "Pallet");
            if (targetPackage != null)
            {
                var assetClasses = MTConnectClassModel.Parse(xmiDocument, "Assets.Pallet", targetPackage.Classes);
                if (assetClasses != null)
                {
                    Pallets.Classes.AddRange(assetClasses);
                }

                //var packages = new List<UmlPackage>();
                //packages.Add(targetPackage);

                //var umlPackages = ModelHelper.GetPackages(targetPackage);
                //var umlClasses = ModelHelper.GetClasses(umlPackages);
                //var assetClasses = MTConnectClassModel.Parse(xmiDocument, "Assets.Pallet", umlClasses);
                //if (assetClasses != null)
                //{
                //    Pallets.Classes.AddRange(assetClasses);
                //}

                // Add Measurement Classes
                var packages = new List<UmlPackage>();
                packages.Add(targetPackage.Packages.FirstOrDefault(o => o.Name == "Measurements"));

                var umlClasses = ModelHelper.GetClasses(packages);
                var measurementClasses = MTConnectClassModel.Parse(xmiDocument, "Assets.Pallet", umlClasses);
                if (measurementClasses != null)
                {
                    foreach (var measurementClass in measurementClasses)
                    {
                        if (measurementClass.Id != "Assets.Pallet.Measurement")
                        {
                            measurementClass.Id = $"{measurementClass.Id}Measurement";
                            measurementClass.Name = $"{measurementClass.Name}Measurement";
                        }
                    }

                    Pallets.Classes.AddRange(measurementClasses);
                }


                // Add Enums
                //var profile = xmiDocument.Model.Profiles.FirstOrDefault();
                //var dataTypes = profile.Packages.FirstOrDefault(o => o.Name == "DataTypes");

                //Pallets.Enums.Add(new MTConnectEnumModel(xmiDocument, "Assets.Pallet.Measurements", dataTypes?.Enumerations.FirstOrDefault(o => o.Name == "CodeEnum"), ConvertMeasurementCode));
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
