using MTConnect.SysML.Xmi;
using MTConnect.SysML.Xmi.UML;
using Scriban;
using System;
using System.IO;
using System.Linq;

namespace MTConnect.SysML.CSharp
{
    internal class ClassModel : MTConnectClassModel, ITemplateModel
    {
        public string Namespace => NamespaceHelper.GetNamespace(Id);

        public bool IsPartial { get; set; }

        public string MaximumVersionEnum => MTConnectVersion.GetVersionEnum(MaximumVersion);

        public string MinimumVersionEnum => MTConnectVersion.GetVersionEnum(MinimumVersion);


        public ClassModel() { }

        public ClassModel(XmiDocument xmiDocument, string id, UmlClass umlClass) : base(xmiDocument, id, umlClass) { }


        public static ClassModel Create(MTConnectClassModel importModel)
        {
            if (importModel != null)
            {
                var type = typeof(ClassModel);

                var importProperties = importModel.GetType().GetProperties();
                var exportProperties = type.GetProperties();

                if (importProperties != null && exportProperties != null)
                {
                    var exportModel = new ClassModel();

                    foreach (var importProperty in importProperties)
                    {
                        var propertyValue = importProperty.GetValue(importModel);

                        var exportProperty = exportProperties.FirstOrDefault(o => o.Name ==  importProperty.Name);
                        if (exportProperty != null)
                        {
                            exportProperty.SetValue(exportModel, propertyValue);
                        }
                    }

                    foreach (var propertyModel in exportModel.Properties)
                    {
                        //// Convert to Interface
                        //if (propertyModel.DataType.ToLower() != propertyModel.DataType && propertyModel.DataType != "System.DateTime" && !propertyModel.DataType.EndsWith("Enum"))
                        //{
                        //    if (!propertyModel.DataType.StartsWith("I")) propertyModel.DataType = $"I{propertyModel.DataType}";
                        //    //if (!propertyModel.DataType.StartsWith("I")) propertyModel.DataType = $"I{propertyModel.DataType}";
                        //}

                        // Remove 'Enum' suffix
                        if (propertyModel.DataType.EndsWith("Enum"))
                        {   
                            var suffix = "Enum";
                            if (propertyModel.DataType.EndsWith(suffix)) propertyModel.DataType = propertyModel.DataType.Substring(0, propertyModel.DataType.Length - suffix.Length);
                        }
                    }

                    return exportModel;
                }
            }

            return null;
        }


        public string RenderModel()
        {
            var templateFilename = $"Model.scriban";
            var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "csharp", "templates", templateFilename);
            if (File.Exists(templatePath))
            {
                try
                {
                    var templateContents = File.ReadAllText(templatePath);
                    if (templateContents != null)
                    {
                        var template = Template.Parse(templateContents);
                        return template.Render(this);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return null;
        }

        public string RenderInterface()
        {
            var templateFilename = $"Interface.scriban";
            var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "csharp", "templates", templateFilename);
            if (File.Exists(templatePath))
            {
                try
                {
                    var templateContents = File.ReadAllText(templatePath);
                    if (templateContents != null)
                    {
                        var template = Template.Parse(templateContents);
                        return template.Render(this);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return null;
        }

        public string RenderDescriptions()
        {
            var templateFilename = $"ModelDescriptions.scriban";
            var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "csharp", "templates", templateFilename);
            if (File.Exists(templatePath))
            {
                try
                {
                    var templateContents = File.ReadAllText(templatePath);
                    if (templateContents != null)
                    {
                        var template = Template.Parse(templateContents);
                        return template.Render(this);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return null;
        }
    }
}
