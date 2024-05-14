// Copyright(c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using MTConnect.Input;
using MTConnect.Logging;

namespace MTConnect.Agents
{
    public abstract class MTConnectAgentProcessor : IMTConnectAgentProcessor
    {
        public string Id { get; set; }

        public string Description { get; set; }


        public event MTConnectLogEventHandler LogReceived;


        public virtual void Load() { }


        public IObservationInput Process(ProcessObservation observation) => OnProcess(observation);

        public IAsset Process(IAsset asset) => OnProcess(asset);


        protected virtual IObservationInput OnProcess(ProcessObservation observation)
        {
            if (observation != null)
            {
                var observationInput = new ObservationInput();
                observationInput.DeviceKey = observation.DataItem?.Device?.Uuid;
                observationInput.DataItemKey = observation.DataItem?.Id;
                observationInput.Timestamp = observation.Timestamp.ToUnixTime();
                observationInput.Values = observation.Values;
                return observationInput;
            }

            return null;
        }

        protected virtual IAsset OnProcess(IAsset asset) => asset;


        protected void Log(MTConnectLogLevel logLevel, string message)
        {
            if (LogReceived != null) LogReceived.Invoke(this, logLevel, message);
        }
    }
}
