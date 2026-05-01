using MTConnect.NET_SysML_Import.CSharp;
using MTConnect.SysML.Models.Assets;

namespace MTConnect.SysML.CSharp
{
    public class MeasurementModel : MTConnectMeasurementModel, ITemplateModel
    {
        public string Namespace => NamespaceHelper.GetNamespace(Id);

        public string XmlDescription { get; set; }


        public static MeasurementModel Create(MTConnectMeasurementModel importModel)
        {
            if (importModel != null)
            {
                var type = typeof(MeasurementModel);

                var importProperties = importModel.GetType().GetProperties();
                var exportProperties = type.GetProperties();

                if (importProperties != null && exportProperties != null)
                {
                    var exportModel = new MeasurementModel();

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
            var template = TemplateLoader.LoadOrThrow("CSharp", "Templates", "Pallets.Measurement.scriban");
            return template.Render(this);
        }

        public string RenderInterface() => null;

        public string RenderDescriptions() => null;
    }
}
