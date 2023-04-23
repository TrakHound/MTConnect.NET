using MTConnect.Assets;
using MTConnect.Devices;
using MTConnect.Observations;
using System;

namespace MTConnect.Clients
{
    public interface IMTConnectEntityClient
    {
        EventHandler<IDevice> DeviceReceived { get; set; }

        EventHandler<IAsset> AssetReceived { get; set; }

        EventHandler<IObservation> ObservationReceived { get; set; }


        void Start();

        void Stop();
    }
}
