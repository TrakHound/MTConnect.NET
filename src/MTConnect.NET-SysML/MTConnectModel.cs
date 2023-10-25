using MTConnect.SysML.Models.Assets;
using MTConnect.SysML.Models.Devices;
using MTConnect.SysML.Models.Observations;
using MTConnect.SysML.Xmi;
using System.Threading;

namespace MTConnect.SysML
{
    public class MTConnectModel
    {
        public MTConnectDeviceInformationModel DeviceInformationModel { get; set; }

        public MTConnectObservationInformationModel ObservationInformationModel { get; set; }

        public MTConnectAssetInformationModel AssetInformationModel { get; set; }

        public MTConnectInterfaceInformationModel IntefaceInformationModel { get; set; }


        public static MTConnectModel Parse(string xmiPath)
        {
            if (!string.IsNullOrEmpty(xmiPath))
            {
                var deserializer = XmiDeserializer.FromFile(xmiPath);
                var doc = deserializer.Deserialize(CancellationToken.None);
                if (doc != null)
                {
                    var mtconnectModel = new MTConnectModel();

                    mtconnectModel.DeviceInformationModel = new MTConnectDeviceInformationModel(doc);
                    mtconnectModel.ObservationInformationModel = new MTConnectObservationInformationModel(doc);
                    mtconnectModel.AssetInformationModel = new MTConnectAssetInformationModel(doc);
                    mtconnectModel.IntefaceInformationModel = new MTConnectInterfaceInformationModel(doc);

                    return mtconnectModel;
                }
            }

            return null;
        }
    }
}
