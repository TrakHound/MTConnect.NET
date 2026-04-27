using MTConnect.NET_SysML_Import.CSharp;
using MTConnect.SysML.Models.Assets;
using MTConnect.SysML.Models.Observations;
using System.Linq;

namespace MTConnect.SysML.CSharp
{
    public class DataSetResultModel : MTConnectClassModel, ITemplateModel
    {
        public string Namespace => NamespaceHelper.GetNamespace(Id);

        public string XmlDescription { get; set; }


        public static DataSetResultModel Create(MTConnectClassModel importModel)
        {
            if (importModel != null)
            {
                var type = typeof(MTConnectClassModel);

                var importProperties = importModel.GetType().GetProperties();
                var exportProperties = type.GetProperties();

                if (importProperties != null && exportProperties != null)
                {
                    var exportModel = new DataSetResultModel();

                    foreach (var importProperty in importProperties)
                    {
                        var propertyValue = importProperty.GetValue(importModel);

                        var exportProperty = exportProperties.FirstOrDefault(o => o.Name == importProperty.Name);
                        if (exportProperty != null && exportProperty.PropertyType == importProperty.PropertyType)
                        {
                            exportProperty.SetValue(exportModel, propertyValue);
                        }
                    }

                    foreach (var exportProperty in exportModel.Properties)
                    {
                        exportProperty.DataType = ModelHelper.RemoveEnumSuffix(exportProperty.DataType);
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
            var template = TemplateLoader.LoadOrThrow("CSharp", "Templates", "Observations.DataSetResults.scriban");
            return template.Render(this);
        }

        public string RenderInterface() => null;

        public string RenderDescriptions() => null;
    }
}
