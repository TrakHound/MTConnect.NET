using MTConnect.SysML.Xmi;
using System.Linq;

namespace MTConnect.SysML.Models.Devices
{
    public class MTConnectInterfaceInformationModel
    {
        public MTConnectDataItemsModel DataItems { get; private set; }


        public MTConnectInterfaceInformationModel() { }

        public MTConnectInterfaceInformationModel(XmiDocument xmiDocument)
        {
            Parse(xmiDocument);
        }


        private void Parse(XmiDocument xmiDocument)
        {
            if (xmiDocument != null)
            {
                var umlModel = xmiDocument.Model;

                // Find Interface Information Model in the UML
                var interfaceInformationModel = umlModel.Packages.FirstOrDefault(o => o.Name == "Interface Interaction Model");
                var observationInformationModel = umlModel.Packages.FirstOrDefault(o => o.Name == "Observation Information Model");
                if (interfaceInformationModel != null && observationInformationModel != null)
                {
                    // DataItems
                    var deviceDataItems = interfaceInformationModel.Packages?.FirstOrDefault(o => o.Name == "DataItem Types for Interface");
                    if (deviceDataItems != null)
                    {
                        DataItems = new MTConnectDataItemsModel();

                        var conditionEnum = umlModel.Profiles.FirstOrDefault().Packages.FirstOrDefault().Enumerations.FirstOrDefault(o => o.Name == "InterfaceEventEnum");
                        DataItems.Types.AddRange(MTConnectInterfaceDataItemType.Parse(xmiDocument, "EVENT", "Interfaces", deviceDataItems.Classes, conditionEnum));
                    }
                }
            }
        }
    }
}
