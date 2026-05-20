using MTConnect.SysML.Xmi;
using MTConnect.SysML.Xmi.UML;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.SysML.Models.Assets
{
    /// <summary>
    /// Parses the Asset Information Model from the XMI: the <c>Asset</c> and
    /// <c>PhysicalAsset</c> base classes plus the per-asset-family packages
    /// (cutting tools, files, fixtures, pallets, QIF, raw materials,
    /// component configuration parameters).
    /// </summary>
    public class MTConnectAssetInformationModel
    {
        /// <summary>
        /// The parsed <c>Asset</c> base class.
        /// </summary>
        public MTConnectAssetModel Asset { get; set; }

        /// <summary>
        /// The parsed <c>PhysicalAsset</c> base class.
        /// </summary>
        public MTConnectPhysicalAssetModel PhysicalAsset { get; set; }

        /// <summary>
        /// The parsed Component Configuration Parameters asset classes.
        /// </summary>
        public MTConnectPackageModel ComponentConfigurationParameters { get; set; } = new();

        /// <summary>
        /// The parsed Cutting Tool asset classes, measurement classes, and
        /// enumerations.
        /// </summary>
        public MTConnectPackageModel CuttingTools { get; set; } = new();

        /// <summary>
        /// The parsed File asset classes and enumerations.
        /// </summary>
        public MTConnectPackageModel Files { get; set; } = new();

        /// <summary>
        /// The parsed Fixture asset classes.
        /// </summary>
        public MTConnectPackageModel Fixtures { get; set; } = new();

        /// <summary>
        /// The parsed Pallet asset classes and measurement classes.
        /// </summary>
        public MTConnectPackageModel Pallets { get; set; } = new();

        /// <summary>
        /// The parsed QIF asset classes and enumerations.
        /// </summary>
        public MTConnectPackageModel QIF { get; set; } = new();

        /// <summary>
        /// The parsed Raw Material asset classes and enumerations.
        /// </summary>
        public MTConnectPackageModel RawMaterials { get; set; } = new();


        /// <summary>
        /// Creates an empty model for manual population.
        /// </summary>
        public MTConnectAssetInformationModel() { }

        /// <summary>
        /// Parses the full asset information model from the given XMI
        /// document.
        /// </summary>
        public MTConnectAssetInformationModel(XmiDocument xmiDocument)
        {
            Parse(xmiDocument);
        }


        /// <summary>
        /// Locates the Asset Information Model package and parses the asset
        /// base classes and every per-family package from it.
        /// </summary>
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


        /// <summary>
        /// Parses the Component Configuration Parameters package into
        /// <see cref="ComponentConfigurationParameters"/>.
        /// </summary>
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

        /// <summary>
        /// Parses the Cutting Tool package into <see cref="CuttingTools"/>:
        /// its primary classes, the rich measurement subtypes, and the
        /// cutting-tool enumerations.
        /// </summary>
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


                // Emit the shared abstract `Measurement` base under
                // Assets.CuttingTools as well. The SysML model declares a
                // single `Measurement` class (carried under the Pallet
                // Measurements package); the CuttingTool measurement
                // hierarchy reuses it as the concrete base of
                // `ToolingMeasurement` and the per-subtype rich measurement
                // classes. Without this second emission the generated
                // `ToolingMeasurement.g.cs` (`: Measurement`) has no
                // `MTConnect.Assets.CuttingTools.Measurement` base to compile
                // against, and the hand-authored
                // `Assets/CuttingTools/Measurement.cs` partial has no
                // generated partner supplying the `IMeasurement` members.
                // Mirrors the abstract-`Measurement` handling in
                // ParsePallets; the renderer's `Assets.CuttingTools.Measurement`
                // case re-emits it as a concrete partial.
                var palletPackage = umlPackage.Packages.FirstOrDefault(o => o.Name == "Pallet");
                var palletMeasurements = palletPackage?.Packages.FirstOrDefault(o => o.Name == "Measurements");
                var sharedMeasurement = palletMeasurements?.Classes.Where(o => o.Name == "Measurement");
                if (sharedMeasurement != null)
                {
                    var baseMeasurementClasses = MTConnectClassModel.Parse(xmiDocument, "Assets.CuttingTools", sharedMeasurement);
                    if (baseMeasurementClasses != null)
                    {
                        CuttingTools.Classes.AddRange(baseMeasurementClasses);
                    }
                }
            }
        }

        /// <summary>
        /// Parses the Files package into <see cref="Files"/> along with its
        /// application/file-state enumerations.
        /// </summary>
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

        /// <summary>
        /// Parses the Fixture package into <see cref="Fixtures"/>.
        /// </summary>
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

        /// <summary>
        /// Parses the Pallet package into <see cref="Pallets"/>: its
        /// primary classes, the concrete measurement subclasses through the
        /// rich measurement pipeline, and the abstract <c>Measurement</c>
        /// base as a partial class.
        /// </summary>
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

                // Add Measurement Classes via the rich measurement pipeline
                // (mirrors the CuttingTool path at line 119) so each Pallet
                // measurement subclass renders with TypeId / CodeId /
                // three-ctor scaffolding rather than the bare ClassModel
                // shape. Filter out the abstract `Measurement` class itself
                // (MTConnectMeasurementModel auto-suffixes "Measurement",
                // which would otherwise produce `MeasurementMeasurement`).
                var packages = new List<UmlPackage>();
                packages.Add(targetPackage.Packages.FirstOrDefault(o => o.Name == "Measurements"));

                var allMeasurementClasses = ModelHelper.GetClasses(packages);
                var concreteMeasurementClasses = allMeasurementClasses
                    ?.Where(c => c.Name != "Measurement");
                var measurementClasses = MTConnectMeasurementModel.Parse(
                    xmiDocument, "PhysicalAsset", "Assets.Pallet", concreteMeasurementClasses);
                if (measurementClasses != null)
                {
                    Pallets.Classes.AddRange(measurementClasses);
                }

                // Also parse the abstract `Measurement` base class itself via
                // the regular class pipeline so it can be regenerated as a
                // partial class (see TemplateRenderer override). The concrete
                // subclasses' `Measurement(IMeasurement)` ctor needs the base
                // partial to provide the copy-from-IMeasurement ctor + the
                // `Type` property the rich template emits.
                var abstractMeasurement = allMeasurementClasses
                    ?.Where(c => c.Name == "Measurement");
                if (abstractMeasurement != null)
                {
                    var baseMeasurementClasses = MTConnectClassModel.Parse(
                        xmiDocument, "Assets.Pallet", abstractMeasurement);
                    if (baseMeasurementClasses != null)
                    {
                        Pallets.Classes.AddRange(baseMeasurementClasses);
                    }
                }


                // Add Enums
                //var profile = xmiDocument.Model.Profiles.FirstOrDefault();
                //var dataTypes = profile.Packages.FirstOrDefault(o => o.Name == "DataTypes");

                //Pallets.Enums.Add(new MTConnectEnumModel(xmiDocument, "Assets.Pallet.Measurements", dataTypes?.Enumerations.FirstOrDefault(o => o.Name == "CodeEnum"), ConvertMeasurementCode));
            }
        }

        /// <summary>
        /// Parses the QIF package into <see cref="QIF"/> along with its
        /// document-type enumeration.
        /// </summary>
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

        /// <summary>
        /// Parses the Raw Material package into <see cref="RawMaterials"/>
        /// along with its form enumeration.
        /// </summary>
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


        /// <summary>
        /// Maps the cutting-tool measurement code <c>N/A</c> to the
        /// C#-identifier-safe <c>N_A</c>; all other codes pass through.
        /// </summary>
        private static string ConvertMeasurementCode(string name)
        {
            if (name == "N/A") return "N_A";

            return name;
        }
    }
}
