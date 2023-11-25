// Copyright(c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using MTConnect.Devices;
using MTConnect.Input;
using MTConnect.Logging;
using System.Collections.Generic;

namespace MTConnect.Adapters
{
    public abstract class MTConnectAdapterModule : IMTConnectAdapterModule
    {
        public string Id { get; set; }

        public string Description { get; set; }

        public IMTConnectAdapter Adapter { get; set; }


        public event MTConnectLogEventHandler LogReceived;


        public MTConnectAdapterModule(string id)
        {
            Id = id;
        }


        public void Start()
        {
            OnStart();
        }

        public void Stop()
        {
            OnStop();
        }


        protected virtual void OnStart() { }

        protected virtual void OnStop() { }


        public virtual bool AddObservations(IEnumerable<IObservationInput> observations)
        {
            return true;
        }

        public virtual bool AddAssets(IEnumerable<IAssetInput> assets)
        {
            return true;
        }

        public virtual bool AddDevices(IEnumerable<IDevice> devices)
        {
            return true;
        }


        protected void Log(MTConnectLogLevel logLevel, string message)
        {
            if (LogReceived != null) LogReceived.Invoke(this, logLevel, message);
        }
    }
}
