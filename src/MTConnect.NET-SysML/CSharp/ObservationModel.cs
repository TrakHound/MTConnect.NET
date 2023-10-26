using MTConnect.SysML.Models.Observations;
using Scriban;
using System;
using System.IO;
using System.Linq;

namespace MTConnect.SysML.CSharp
{
    public class ObservationModel : MTConnectObservationModel, ITemplateModel
    {
        public string Namespace => NamespaceHelper.GetNamespace(Id);


        public static ObservationModel Create(MTConnectObservationModel importModel)
        {
            if (importModel != null)
            {
                var type = typeof(ObservationModel);

                var importProperties = importModel.GetType().GetProperties();
                var exportProperties = type.GetProperties();

                if (importProperties != null && exportProperties != null)
                {
                    var exportModel = new ObservationModel();

                    foreach (var importProperty in importProperties)
                    {
                        var propertyValue = importProperty.GetValue(importModel);

                        var exportProperty = exportProperties.FirstOrDefault(o => o.Name == importProperty.Name);
                        if (exportProperty != null)
                        {
                            exportProperty.SetValue(exportModel, propertyValue);
                        }
                    }

                    return exportModel;
                }
            }

            return null;
        }

        public string RenderModel()
        {
            var templateFilename = $"Observations.Observation.scriban";
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

        public string RenderInterface() => null;

        public string RenderDescriptions()
        {
            var templateFilename = $"EnumDescriptions.scriban";
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
