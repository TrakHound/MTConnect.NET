using MTConnect.Assets;
using MTConnect.Devices;
using MTConnect.Observations;
using System;

namespace MTConnect.Clients
{
    public interface IMTConnectEntityClient
    {
        event EventHandler<IDevice> DeviceReceived;

        event EventHandler<IAsset> AssetReceived;

        event EventHandler<IObservation> ObservationReceived;


        void Start();

        void Stop();
    }
}
