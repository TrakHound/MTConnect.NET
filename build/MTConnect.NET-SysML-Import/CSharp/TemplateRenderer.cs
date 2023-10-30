using MTConnect.SysML.Models.Assets;
using MTConnect.SysML.Models.Devices;
using MTConnect.SysML.Models.Observations;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
                    var dClassModels = classModels.Where(o => o.Name != null).ToDictionary(o => o.Name);

                    var enumModels = exportModels.Where(o => typeof(MTConnectEnumModel).IsAssignableFrom(o.GetType())).Select(o => (MTConnectEnumModel)o);
                    var dEnumModels = enumModels.Where(o => o.Name != null).ToDictionary(o => o.Name);


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
                        else if (typeof(MTConnectComponentType).IsAssignableFrom(type)) template = ComponentType.Create((MTConnectComponentType)exportModel);
                        else if (typeof(MTConnectCuttingToolMeasurementModel).IsAssignableFrom(type)) template = CuttingToolMeasurementModel.Create((MTConnectCuttingToolMeasurementModel)exportModel);
                        else if (typeof(MTConnectClassModel).IsAssignableFrom(type)) template = ClassModel.Create((MTConnectClassModel)exportModel);
                        else if (typeof(MTConnectObservationModel).IsAssignableFrom(type)) template = ObservationModel.Create((MTConnectObservationModel)exportModel);
                        else if (typeof(MTConnectEnumModel).IsAssignableFrom(type))
                        {
                            switch (exportModel.Id)
                            {
                                case "Devices.UnitsEnum": template = EnumStringModel.Create((MTConnectEnumModel)exportModel, ConvertUnitEnum); break;
                                case "Devices.NativeUnitsEnum": template = EnumStringModel.Create((MTConnectEnumModel)exportModel, ConvertUnitEnum); break;
                                case "Assets.CuttingTools.Measurements.MeasurementCodeEnum": template = EnumStringModel.Create((MTConnectEnumModel)exportModel, ConvertMeasurementCodeEnum); break;
                                default: template = EnumModel.Create((MTConnectEnumModel)exportModel); break;
                            }
                        }

                        if (template != null)
                        {
                            switch (template.Id)
                            {
                                case "Devices.Device": 
                                    ((ClassModel)template).IsPartial = true;
                                    ((ClassModel)template).ParentName = null;
                                    break;

                                case "Devices.Component": ((ClassModel)template).IsPartial = true; break;
                                case "Devices.Composition": ((ClassModel)template).IsPartial = true; break;
                                case "Devices.DataItem": ((ClassModel)template).IsPartial = true; break;
                                case "Devices.AbstractDataItemRelationship": ((ClassModel)template).IsPartial = true; break;
                                case "Devices.References.Reference": ((ClassModel)template).IsPartial = true; break;
                                case "Devices.References.ComponentReference": ((ClassModel)template).HasDescriptions = false; break;
                                case "Devices.References.DataItemReference": ((ClassModel)template).HasDescriptions = false; break;
                                case "Devices.Units": ((EnumStringModel)template).IsPartial = true; break;

                                //case "Devices.DataItemResetTrigger": ((EnumModel)template).Values.Add(new MTConnectEnumValueModel { Name = "NONE" }); break;
                                //case "Devices.DataItemStatistic": ((EnumModel)template).Values.Add(new MTConnectEnumValueModel { Name = "NONE" }); break;
                                //case "Devices.Configurations.CriticalityType": ((EnumModel)template).Values.Add(new MTConnectEnumValueModel { Name = "NOT_SPECIFIED" }); break;

                                case "Assets.Asset": ((ClassModel)template).IsPartial = true; break;
                                case "Assets.ComponentConfigurationParameters.ComponentConfigurationParameters": 
                                    ((ClassModel)template).IsPartial = true;
                                    ((ClassModel)template).Id += "Asset";
                                    ((ClassModel)template).Name += "Asset";
                                    if (((ClassModel)template).ParentName != null && ((ClassModel)template).ParentName != "Asset") ((ClassModel)template).ParentName += "Asset";
                                    break;
                                case "Assets.CuttingTools.CuttingTool": 
                                    ((ClassModel)template).IsPartial = true;
                                    ((ClassModel)template).Id += "Asset";
                                    ((ClassModel)template).Name += "Asset";
                                    if (((ClassModel)template).ParentName != null && ((ClassModel)template).ParentName != "Asset") ((ClassModel)template).ParentName += "Asset";
                                    break;
                                case "Assets.CuttingTools.CuttingToolArchetype":
                                    ((ClassModel)template).IsPartial = true;
                                    ((ClassModel)template).Id += "Asset";
                                    ((ClassModel)template).Name += "Asset";
                                    if (((ClassModel)template).ParentName != null && ((ClassModel)template).ParentName != "Asset") ((ClassModel)template).ParentName += "Asset";
                                    break;
                                case "Assets.CuttingTools.CuttingToolLifeCycle": ((ClassModel)template).IsPartial = true; break;
                                case "Assets.CuttingTools.CuttingItem": ((ClassModel)template).IsPartial = true; break;
                                case "Assets.CuttingTools.Measurement":
                                    ((ClassModel)template).IsPartial = true; 
                                    ((ClassModel)template).IsAbstract = false; 
                                    break;
                                case "Assets.Files.File": 
                                    ((ClassModel)template).IsPartial = true;
                                    ((ClassModel)template).Id += "Asset";
                                    ((ClassModel)template).Name += "Asset";
                                    if (((ClassModel)template).ParentName != null && ((ClassModel)template).ParentName != "Asset") ((ClassModel)template).ParentName += "Asset";
                                    break;
                                case "Assets.Files.FileArchetype":
                                    ((ClassModel)template).IsPartial = true;
                                    ((ClassModel)template).Id += "Asset";
                                    ((ClassModel)template).Name += "Asset";
                                    if (((ClassModel)template).ParentName != null && ((ClassModel)template).ParentName != "Asset") ((ClassModel)template).ParentName += "Asset";
                                    break;
                                case "Assets.Files.AbstractFile":
                                    ((ClassModel)template).IsPartial = true;
                                    ((ClassModel)template).Id += "Asset";
                                    ((ClassModel)template).Name += "Asset";
                                    if (((ClassModel)template).ParentName != null && ((ClassModel)template).ParentName != "Asset") ((ClassModel)template).ParentName += "Asset";
                                    break;
                                case "Assets.QIF.QIFDocumentWrapper":
                                    ((ClassModel)template).IsPartial = true;
                                    ((ClassModel)template).Id += "Asset";
                                    ((ClassModel)template).Name += "Asset";
                                    if (((ClassModel)template).ParentName != null && ((ClassModel)template).ParentName != "Asset") ((ClassModel)template).ParentName += "Asset";
                                    break;
                                case "Assets.RawMaterials.RawMaterial":
                                    ((ClassModel)template).IsPartial = true;
                                    ((ClassModel)template).Id += "Asset";
                                    ((ClassModel)template).Name += "Asset";
                                    if (((ClassModel)template).ParentName != null && ((ClassModel)template).ParentName != "Asset") ((ClassModel)template).ParentName += "Asset";
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
                            foreach (var property in ((ClassModel)componentModel).Properties?.Where(o => o.Name != "Components" && o.Name != "Compositions"))
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
                            foreach (var property in ((ClassModel)componentModel).Properties?.Where(o => o.Name != "Components" && o.Name != "Compositions"))
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
            var exportModels = new List<IMTConnectExportModel>();

            if (model != null)
            {
                var modelType = model.GetType();

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
                            if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                            {
                                IEnumerable childValues = (IEnumerable)propertyValue;
                                foreach (var childValue in childValues)
                                {
                                    exportModels.AddRange(GetExportModels(childValue));
                                }
                            }
                            else
                            {
                                exportModels.AddRange(GetExportModels(propertyValue));
                            }
                        }
                    }
                }
            }

            return exportModels;
        }


        private static void WriteModel(ITemplateModel template, string outputPath)
        {
            if (template != null)
            {
                var result = template.RenderModel();
                if (result != null && template.Id != null)
                {
                    var resultPath = template.Id.Replace('.', '\\');
                    resultPath = Path.Combine(outputPath, resultPath);
                    resultPath = $"{resultPath}.g.cs";

                    var resultDirectory = Path.GetDirectoryName(resultPath);
                    if (!Directory.Exists(resultDirectory)) Directory.CreateDirectory(resultDirectory);

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
                    var resultPath = template.Id.Replace('.', '\\');
                    resultPath = Path.Combine(outputPath, resultPath);
                    var resultDirectory = Path.GetDirectoryName(resultPath);
                    var resultFilename = Path.GetFileName(resultPath);
                    resultPath = Path.Combine(resultDirectory, $"I{resultFilename}.g.cs");

                    if (!Directory.Exists(resultDirectory)) Directory.CreateDirectory(resultDirectory);

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
                    var resultPath = template.Id.Replace('.', '\\');
                    resultPath = Path.Combine(outputPath, resultPath);
                    var resultDirectory = Path.GetDirectoryName(resultPath);
                    var resultFilename = Path.GetFileName(resultPath);
                    resultPath = Path.Combine(resultDirectory, $"{resultFilename}Descriptions.g.cs");

                    if (!Directory.Exists(resultDirectory)) Directory.CreateDirectory(resultDirectory);

                    File.WriteAllText(resultPath, result);
                }
            }
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
