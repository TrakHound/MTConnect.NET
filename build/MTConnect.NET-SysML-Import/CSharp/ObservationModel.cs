using MTConnect.NET_SysML_Import.CSharp;
using MTConnect.SysML.Models.Observations;
using System.Linq;

namespace MTConnect.SysML.CSharp
{
    public class ObservationModel : MTConnectObservationModel, ITemplateModel
    {
        public string Namespace => NamespaceHelper.GetNamespace(Id);

        public string XmlDescription { get; set; }


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
                        if (exportProperty != null && exportProperty.PropertyType == importProperty.PropertyType)
                        {
                            exportProperty.SetValue(exportModel, propertyValue);
                        }
                    }

                    exportModel.Description = DescriptionHelper.GetTextDescription(importModel.Description);
                    exportModel.XmlDescription = DescriptionHelper.GetXmlDescription(importModel.Description);

                    return exportModel;
                }
            }

            return null;
        }

        public string RenderModel()
        {
            var template = TemplateLoader.LoadOrThrow("CSharp", "Templates", "Observations.Observation.scriban");
            return template.Render(this);
        }

        public string RenderInterface() => null;

        public string RenderDescriptions()
        {
            var template = TemplateLoader.LoadOrThrow("CSharp", "Templates", "EnumDescriptions.scriban");
            return template.Render(this);
        }
    }
}
