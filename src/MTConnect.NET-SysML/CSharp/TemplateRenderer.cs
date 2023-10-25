using MTConnect.SysML.Models.Devices;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

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

                        if (typeof(MTConnectDataItemType).IsAssignableFrom(type)) template = DataItemType.Create((MTConnectDataItemType)exportModel);
                        else if (typeof(MTConnectCompositionType).IsAssignableFrom(type)) template = CompositionType.Create((MTConnectCompositionType)exportModel);
                        else if (typeof(MTConnectComponentType).IsAssignableFrom(type)) template = ComponentType.Create((MTConnectComponentType)exportModel);
                        else if (typeof(MTConnectClassModel).IsAssignableFrom(type)) template = ClassModel.Create((MTConnectClassModel)exportModel);
                        else if (typeof(MTConnectEnumModel).IsAssignableFrom(type))
                        {
                            switch (exportModel.Id)
                            {
                                case "Devices.UnitsEnum": template = EnumStringModel.Create((MTConnectEnumModel)exportModel, ConvertUnitEnum); break;
                                case "Devices.NativeUnitsEnum": template = EnumStringModel.Create((MTConnectEnumModel)exportModel, ConvertUnitEnum); break;
                                case "Assets.CuttingTools.Measurements.MeasurementCodeEnum": template = EnumStringModel.Create((MTConnectEnumModel)exportModel); break;
                                default: template = EnumModel.Create((MTConnectEnumModel)exportModel); break;
                            }
                        }

                        if (template != null)
                        {
                            switch (exportModel.Id)
                            {
                                case "Devices.Device": ((ClassModel)template).IsPartial = true; break;
                                case "Devices.Component": ((ClassModel)template).IsPartial = true; break;
                                case "Devices.Composition": ((ClassModel)template).IsPartial = true; break;
                                case "Devices.DataItem": ((ClassModel)template).IsPartial = true; break;
                                case "Assets.Asset": ((ClassModel)template).IsPartial = true; break;
                            }

                            templates.Add(template);
                        }
                    }

                    if (templates != null)
                    {
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
    }
}
