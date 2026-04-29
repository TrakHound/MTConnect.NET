using MTConnect.SysML.Models.Assets;
using MTConnect.SysML.Models.Devices;
using MTConnect.SysML.Models.Observations;
using System.Collections;

namespace MTConnect.SysML.CSharp
{
    public static class CSharpTemplateRenderer
    {
        public static void Render(MTConnectModel mtconnectModel, string outputPath)
        {
            if (mtconnectModel != null && !string.IsNullOrEmpty(outputPath))
            {
                var exportModels = GetExportModels(mtconnectModel);
                if (exportModels != null)
                {
                    var classModels = exportModels.Where(o => typeof(MTConnectClassModel).IsAssignableFrom(o.GetType())).Select(o => (MTConnectClassModel)o);

                    var dClassModels = new Dictionary<string, MTConnectClassModel>();
                    foreach (var classModel in classModels)
                    {
                        if (!string.IsNullOrEmpty(classModel.Name))
                        {
                            // TryAdd preserves first-wins semantics with a single hash lookup.
                            dClassModels.TryAdd(classModel.Name, classModel);
                        }
                    }


                    var enumModels = exportModels.Where(o => typeof(MTConnectEnumModel).IsAssignableFrom(o.GetType())).Select(o => (MTConnectEnumModel)o);

                    var dEnumModels = new Dictionary<string, MTConnectEnumModel>();
                    foreach (var enumModel in enumModels)
                    {
                        if (!string.IsNullOrEmpty(enumModel.Name))
                        {
                            dEnumModels.TryAdd(enumModel.Name, enumModel);
                        }
                    }


                    var templates = new List<ITemplateModel>();

                    foreach (var exportModel in exportModels)
                    {
                        var type = exportModel.GetType();

                        if (typeof(MTConnectClassModel).IsAssignableFrom(type))
                        {
                            var classModel = (MTConnectClassModel)exportModel;
                            if (classModel.Properties != null)
                            {
                                foreach (var property in classModel.Properties)
                                {
                                    switch (property.DataType)
                                    {
                                        case "UnitsEnum": property.DataType = "string"; break;
                                        case "NativeUnitsEnum": property.DataType = "string"; break;
                                        case "MeasurementCodeEnum": property.DataType = "string"; break;
                                        case "UNIT_VECTOR_3D": property.DataType = "MTConnect.UnitVector3D"; break;
                                        case "POSITION_3D": property.DataType = "MTConnect.Position3D"; break;
                                        case "DEGREE_3D": property.DataType = "MTConnect.Degree3D"; break;
                                        case "MILLIMETER_3D": property.DataType = "MTConnect.Millimeter3D"; break;
                                        case "QIFDocument": property.DataType = "string"; break;
                                        default:

                                            var classMatch = dClassModels.GetValueOrDefault(property.DataType);
                                            if (classMatch != null)
                                            {
                                                property.DataType = $"{NamespaceHelper.GetNamespace(classMatch.Id)}.I{classMatch.Name}";
                                            }
                                            else
                                            {
                                                var enumMatch = dEnumModels.GetValueOrDefault(property.DataType);
                                                if (enumMatch != null)
                                                {
                                                    property.DataType = $"{NamespaceHelper.GetNamespace(enumMatch.Id)}.{enumMatch.Name}";
                                                }
                                            }

                                            break;                               
                                    }
                                }
                            }
                        }


                        ITemplateModel template = null;

                        if (typeof(MTConnectInterfaceDataItemType).IsAssignableFrom(type)) template = InterfaceDataItemType.Create((MTConnectInterfaceDataItemType)exportModel);
                        else if (typeof(MTConnectDataItemType).IsAssignableFrom(type)) template = DataItemType.Create((MTConnectDataItemType)exportModel);
                        else if (typeof(MTConnectCompositionType).IsAssignableFrom(type)) template = CompositionType.Create((MTConnectCompositionType)exportModel);
                        else if (typeof(MTConnectComponentType).IsAssignableFrom(type))
                        {
                            if (((MTConnectComponentType)exportModel).Type == "Controllers") ((MTConnectComponentType)exportModel).MinimumVersion = new Version(1, 0);
                            template = ComponentType.Create((MTConnectComponentType)exportModel);
                        }
                        else if (typeof(MTConnectMeasurementModel).IsAssignableFrom(type))
                        {
                            if (exportModel.Id?.StartsWith("Assets.CuttingTools.") == true)
                            {
                                template = CuttingToolMeasurementModel.Create((MTConnectMeasurementModel)exportModel);
                            }
                            else
                            {
                                // Non-CuttingTools measurement (e.g. Assets.Pallet.*) — no fallback
                                // template exists yet, so log and continue rather than silently
                                // dropping the model.
                                Console.Error.WriteLine(
                                    $"warn: MeasurementModel '{exportModel.Id}' has no template — " +
                                    "only Assets.CuttingTools.* is currently rendered. Skipping.");
                            }
                        }
                        else if (typeof(MTConnectClassModel).IsAssignableFrom(type) && exportModel.Id?.EndsWith("Result") == true)
                        {
                            // Suffix-based DataSetResult selector. Type guard required because the recursive
                            // GetExportModels walk surfaces both classes AND properties; a property whose Id
                            // happens to end in "Result" (e.g. `Devices.Configurations.DataSet.Result` — the
                            // `result` field on the v2.7 DataSet base class) would otherwise crash with
                            // InvalidCastException when forced into MTConnectClassModel.
                            template = DataSetResultModel.Create((MTConnectClassModel)exportModel);
                        }
                        else if (typeof(MTConnectClassModel).IsAssignableFrom(type)) template = ClassModel.Create((MTConnectClassModel)exportModel);
                        else if (typeof(MTConnectObservationModel).IsAssignableFrom(type)) template = ObservationModel.Create((MTConnectObservationModel)exportModel);
                        else if (typeof(MTConnectEnumModel).IsAssignableFrom(type))
                        {
                            switch (exportModel.Id)
                            {
                                case "Devices.UnitsEnum": template = EnumStringModel.Create((MTConnectEnumModel)exportModel, ConvertUnitEnum); break;
                                case "Devices.NativeUnitsEnum": template = EnumStringModel.Create((MTConnectEnumModel)exportModel, ConvertUnitEnum); break;
                                case "Assets.CuttingTools.Measurements.MeasurementCodeEnum": template = EnumStringModel.Create((MTConnectEnumModel)exportModel, ConvertMeasurementCodeEnum); break;
                                //case "Assets.Pallet.Measurements.MeasurementCodeEnum": template = EnumStringModel.Create((MTConnectEnumModel)exportModel, ConvertMeasurementCodeEnum); break;
                                default: template = EnumModel.Create((MTConnectEnumModel)exportModel); break;
                            }
                        }


                        if (template != null)
                        {
                            switch (template.Id)
                            {
                                case "Devices.Device":
                                    // Devices.Device's concrete C# base is wiped here; Component
                                    // is reintroduced through the hand-authored partial-class
                                    // file. Additional generalizations are cleared too so that
                                    // none of them survive into the generated header.
                                    ((ClassModel)template).IsPartial = true;
                                    ((ClassModel)template).ParentName = null;
                                    ((ClassModel)template).AdditionalParentNames = new System.Collections.Generic.List<string>();
                                    ((ClassModel)template).AdditionalParentUmlIds = new System.Collections.Generic.List<string>();
                                    break;

                                case "Devices.Component": ((ClassModel)template).IsPartial = true; break;
                                case "Devices.Composition": ((ClassModel)template).IsPartial = true; break;
                                case "Devices.DataItem": ((ClassModel)template).IsPartial = true; break;
                                case "Devices.AbstractDataItemRelationship": ((ClassModel)template).IsPartial = true; break;
                                case "Devices.References.Reference": ((ClassModel)template).IsPartial = true; break;
                                case "Devices.References.ComponentReference": ((ClassModel)template).HasDescriptions = false; break;
                                case "Devices.References.DataItemReference": ((ClassModel)template).HasDescriptions = false; break;
                                case "Devices.Units": ((EnumStringModel)template).IsPartial = true; break;

                                case "Devices.DataItems.ToolOffsetsDataItem": ((DataItemType)template).Representation = "TABLE"; break;
                                case "Devices.DataItems.WorkOffsetsDataItem": ((DataItemType)template).Representation = "TABLE"; break;

                                //case "Devices.DataItemResetTrigger": ((EnumModel)template).Values.Add(new MTConnectEnumValueModel { Name = "NONE" }); break;
                                //case "Devices.DataItemStatistic": ((EnumModel)template).Values.Add(new MTConnectEnumValueModel { Name = "NONE" }); break;
                                //case "Devices.Configurations.CriticalityType": ((EnumModel)template).Values.Add(new MTConnectEnumValueModel { Name = "NOT_SPECIFIED" }); break;

                                case "Assets.Asset": ((ClassModel)template).IsPartial = true; break;
                                case "Assets.PhysicalAsset": ((ClassModel)template).IsPartial = true; break;
                                case "Assets.ComponentConfigurationParameters.ComponentConfigurationParameters":
                                case "Assets.CuttingTools.CuttingTool":
                                case "Assets.CuttingTools.CuttingToolArchetype":
                                case "Assets.Files.File":
                                case "Assets.Files.FileArchetype":
                                case "Assets.Files.AbstractFile":
                                case "Assets.QIF.QIFDocumentWrapper":
                                case "Assets.RawMaterials.RawMaterial":
                                    ApplyAssetSuffix((ClassModel)template, alsoSuffixParent: true);
                                    break;
                                case "Assets.Fixture.Fixture":
                                case "Assets.Pallet.Pallet":
                                    ApplyAssetSuffix((ClassModel)template, alsoSuffixParent: false);
                                    break;
                                case "Assets.CuttingTools.CuttingToolLifeCycle": ((ClassModel)template).IsPartial = true; break;
                                case "Assets.CuttingTools.CuttingItem": ((ClassModel)template).IsPartial = true; break;
                                case "Assets.CuttingTools.ToolLife": ((ClassModel)template).IsPartial = true; break;
                                case "Assets.CuttingTools.Measurement":
                                    ((ClassModel)template).IsPartial = true;
                                    ((ClassModel)template).IsAbstract = false;
                                    break;
                                case "Assets.CuttingTools.ToolingMeasurement":
                                    ((ClassModel)template).IsPartial = true;
                                    ((ClassModel)template).IsAbstract = false;
                                    break;
                            }

                            templates.Add(template);
                        }
                    }

                    if (templates != null)
                    {
                        var componentModel = templates.FirstOrDefault(o => o.Id == "Devices.Component");
                        if (componentModel != null)
                        {
                            // Add special template for the IContainer interface
                            var containerModel = new ClassModel();
                            containerModel.Id = "Devices.Container";
                            containerModel.Name = "Container";
                            containerModel.IsPartial = true;
                            containerModel.HasModel = false;
                            containerModel.HasDescriptions = false;
                            foreach (var property in (((ClassModel)componentModel).Properties ?? Enumerable.Empty<PropertyModel>()).Where(o => o.Name != "Components" && o.Name != "Compositions"))
                            {
                                containerModel.Properties.Add(PropertyModel.Create(property));
                            }
                            templates.Add(containerModel);


                            var deviceModel = templates.FirstOrDefault(o => o.Id == "Devices.Device");
                            if (deviceModel != null)
                            {
                                // Remove redundant Properties (inherits from IComponent)
                                foreach (var property in ((ClassModel)deviceModel).Properties)
                                {
                                    if (property.Name == "Hash") property.ExportToInterface = false;

                                    if (((ClassModel)componentModel).Properties.Any(o => o.Name == property.Name))
                                    {
                                        property.ExportToInterface = false;
                                    }
                                }
                            }

                            var compositionModel = templates.FirstOrDefault(o => o.Id == "Devices.Composition");
                            if (compositionModel != null)
                            {
                                // Remove redundant Properties (inherits from IContainer)
                                foreach (var property in ((ClassModel)compositionModel).Properties)
                                {
                                    if (containerModel.Properties.Any(o => o.Name == property.Name))
                                    {
                                        property.ExportToInterface = false;
                                    }
                                }
                            }

                            // Remove redundant Properties (inherits from IContainer)
                            foreach (var property in (((ClassModel)componentModel).Properties ?? Enumerable.Empty<PropertyModel>()).Where(o => o.Name != "Components" && o.Name != "Compositions"))
                            {
                                property.ExportToInterface = false;
                            }
                        }


                        foreach (var template in templates)
                        {
                            WriteModel(template, outputPath);
                            WriteInterface(template, outputPath);
                            WriteDescriptions(template, outputPath);
                        }
                    }
                }
            }
        }


        private static IEnumerable<IMTConnectExportModel> GetExportModels(object model)
        {
            // Track visited reference-type instances to break cycles. The
            // SysML model graph is generated and can contain back-references
            // (e.g. parent ⇄ child) which would otherwise drive an unbounded
            // recursion → StackOverflowException. HashSet keyed by
            // reference equality so two distinct strings or value-typed
            // boxes don't collide on Equals.
            var visited = new HashSet<object>(ReferenceEqualityComparer.Instance);
            var exportModels = new List<IMTConnectExportModel>();
            CollectExportModels(model, exportModels, visited);
            return exportModels;
        }

        private static void CollectExportModels(object model, List<IMTConnectExportModel> exportModels, HashSet<object> visited)
        {
            if (model == null) return;

            var modelType = model.GetType();

            // Skip primitives, strings, and value types early. Strings are
            // IEnumerable<char> and would otherwise be walked character-by-
            // character, exploding the recursion; value types neither
            // participate in cycles nor implement IMTConnectExportModel.
            if (modelType.IsPrimitive || modelType.IsValueType || modelType == typeof(string)) return;

            if (!visited.Add(model)) return;

            if (typeof(IMTConnectExportModel).IsAssignableFrom(modelType))
            {
                exportModels.Add((IMTConnectExportModel)model);
            }

            var properties = modelType.GetProperties();
            if (properties != null)
            {
                foreach (var property in properties)
                {
                    var propertyValue = property.GetValue(model);
                    if (propertyValue != null)
                    {
                        if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType) && property.PropertyType != typeof(string))
                        {
                            IEnumerable childValues = (IEnumerable)propertyValue;
                            foreach (var childValue in childValues)
                            {
                                CollectExportModels(childValue, exportModels, visited);
                            }
                        }
                        else
                        {
                            CollectExportModels(propertyValue, exportModels, visited);
                        }
                    }
                }
            }
        }


        private static void WriteModel(ITemplateModel template, string outputPath)
        {
            if (template != null)
            {
                var result = template.RenderModel();
                if (result != null && template.Id != null)
                {
                    var resultPath = template.Id.Replace('.', Path.DirectorySeparatorChar);
                    resultPath = Path.Combine(outputPath, resultPath);
                    resultPath = $"{resultPath}.g.cs";

                    var resultDirectory = Path.GetDirectoryName(resultPath);
                    if (!string.IsNullOrEmpty(resultDirectory) && !Directory.Exists(resultDirectory))
                        Directory.CreateDirectory(resultDirectory);

                    File.WriteAllText(resultPath, result);
                }
            }
        }

        private static void WriteInterface(ITemplateModel template, string outputPath)
        {
            if (template != null)
            {
                var result = template.RenderInterface();
                if (result != null && template.Id != null)
                {
                    var resultPath = template.Id.Replace('.', Path.DirectorySeparatorChar);
                    resultPath = Path.Combine(outputPath, resultPath);
                    var resultDirectory = Path.GetDirectoryName(resultPath);
                    var resultFilename = Path.GetFileName(resultPath);
                    resultPath = Path.Combine(resultDirectory ?? string.Empty, $"I{resultFilename}.g.cs");

                    if (!string.IsNullOrEmpty(resultDirectory) && !Directory.Exists(resultDirectory))
                        Directory.CreateDirectory(resultDirectory);

                    File.WriteAllText(resultPath, result);
                }
            }
        }

        private static void WriteDescriptions(ITemplateModel template, string outputPath)
        {
            if (template != null)
            {
                var result = template.RenderDescriptions();
                if (result != null && template.Id != null)
                {
                    var resultPath = template.Id.Replace('.', Path.DirectorySeparatorChar);
                    resultPath = Path.Combine(outputPath, resultPath);
                    var resultDirectory = Path.GetDirectoryName(resultPath);
                    var resultFilename = Path.GetFileName(resultPath);
                    resultPath = Path.Combine(resultDirectory ?? string.Empty, $"{resultFilename}Descriptions.g.cs");

                    if (!string.IsNullOrEmpty(resultDirectory) && !Directory.Exists(resultDirectory))
                        Directory.CreateDirectory(resultDirectory);

                    File.WriteAllText(resultPath, result);
                }
            }
        }

        // Apply the "Asset" suffix to a ClassModel's Id / Name (and optionally
        // ParentName) for cases where the spec collapses the namespace. Guards
        // null Id / Name explicitly — the switch arm in Render guarantees
        // template.Id is the literal spec key, but Name is copied from the
        // imported model and could be null on a malformed XMI; guarding here
        // keeps the suffix from masking a missing Name as the literal "Asset".
        private static void ApplyAssetSuffix(ClassModel template, bool alsoSuffixParent)
        {
            if (template == null) return;
            template.IsPartial = true;
            if (template.Id == null)
                throw new InvalidOperationException("ClassModel has null Id; cannot apply Asset suffix. Asset rename relies on the spec-derived id.");
            template.Id += "Asset";
            if (template.Name == null)
                throw new InvalidOperationException($"ClassModel '{template.Id}' has null Name; cannot apply Asset suffix.");
            template.Name += "Asset";
            if (alsoSuffixParent && template.ParentName != null && template.ParentName != "Asset")
                template.ParentName += "Asset";
        }


        private static string ConvertUnitEnum(string input)
        {           
            var output = input;

            if (output != null)
            {
                output = output.Replace("/", "_PER_");
                output = output.Replace("^2", "_SQUARED");
            }

            return output;
        }

        private static string ConvertMeasurementCodeEnum(string input)
        {
            var output = input;

            if (output != null)
            {
                output = input;
            }

            return output;
        }
    }
}
