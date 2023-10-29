using MTConnect.SysML.Xmi;
using MTConnect.SysML.Xmi.UML;
using System.Linq;

namespace MTConnect.SysML.CSharp
{
    public class PropertyModel : MTConnectPropertyModel
    {
        public bool ExportToModel { get; set; } = true;

        public bool ExportToInterface { get; set; } = true;


        public PropertyModel() { }

        public PropertyModel(XmiDocument xmiDocument, string id, UmlProperty umlProperty) : base(xmiDocument, id, umlProperty) { }


        public static PropertyModel Create(MTConnectPropertyModel importModel)
        {
            if (importModel != null)
            {
                var type = typeof(PropertyModel);

                var importProperties = importModel.GetType().GetProperties();
                var exportProperties = type.GetProperties();

                if (importProperties != null && exportProperties != null)
                {
                    var exportModel = new PropertyModel();

                    foreach (var importProperty in importProperties)
                    {
                        var propertyValue = importProperty.GetValue(importModel);

                        var exportProperty = exportProperties.FirstOrDefault(o => o.Name == importProperty.Name);
                        if (exportProperty != null && exportProperty.PropertyType == importProperty.PropertyType)
                        {
                            exportProperty.SetValue(exportModel, propertyValue);
                        }
                    }

                    return exportModel;
                }
            }

            return null;
        }
    }
}
