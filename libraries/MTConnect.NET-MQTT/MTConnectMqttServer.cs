// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Agents;
using MTConnect.Assets;
using MTConnect.Configurations;
using MTConnect.Devices;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect
{
    public class MTConnectMqttServer
    {
        private readonly IMTConnectAgentBroker _mtconnectAgent;
        private readonly IMTConnectMqttServerConfiguration _configuration;
        private CancellationTokenSource _stop;
        private long _lastSequence;


        public event MTConnectMqttResponseHandler<IDevicesResponseDocument> ProbeReceived;

        public event MTConnectMqttResponseHandler<Streams.Output.IStreamsResponseOutputDocument> CurrentReceived;

        public event MTConnectMqttResponseHandler<Streams.Output.IStreamsResponseOutputDocument> SampleReceived;

        public event MTConnectMqttResponseHandler<IAssetsResponseDocument> AssetReceived;


        public MTConnectMqttServer(IMTConnectAgentBroker mtconnectAgent, IMTConnectMqttServerConfiguration configuration)
        {
            _mtconnectAgent = mtconnectAgent;
            _mtconnectAgent.DeviceAdded += DeviceAdded;
            _mtconnectAgent.AssetAdded += AssetAdded;

            _configuration = configuration;
            if (_configuration == null) _configuration = new MTConnectMqttServerConfiguration();
        }


        public void Start()
        {
            _stop = new CancellationTokenSource();

            var devices = _mtconnectAgent.GetDevices();
            if (!devices.IsNullOrEmpty())
            {
                foreach (var device in devices)
                {
                    var response = _mtconnectAgent.GetDevicesResponseDocument(device.Uuid);
                    if (response != null)
                    {
                        if (ProbeReceived != null) ProbeReceived.Invoke(device, response);
                    }
                }
            }

            _ = Task.Run(CurrentWorker, _stop.Token);
            _ = Task.Run(SampleWorker, _stop.Token);
        }

        public void Stop()
        {
            if (_stop != null) _stop.Cancel();
        }


        private async Task CurrentWorker()
        {
            do
            {
                try
                {
                    var devices = _mtconnectAgent.GetDevices();
                    if (!devices.IsNullOrEmpty())
                    {
                        foreach (var device in devices)
                        {
                            var response = _mtconnectAgent.GetDeviceStreamsResponseDocument(device.Uuid);
                            if (response != null)
                            {
                                if (CurrentReceived != null) CurrentReceived.Invoke(device, response);
                            }
                        }
                    }

                    await Task.Delay(_configuration.CurrentInterval, _stop.Token);
                }
                catch (TaskCanceledException) { }
                catch (Exception ex) { }

            } while (!_stop.Token.IsCancellationRequested);
        }

        private async Task SampleWorker()
        {
            do
            {
                try
                {
                    var devices = _mtconnectAgent.GetDevices();
                    if (!devices.IsNullOrEmpty())
                    {
                        var lastSequence = _lastSequence;

                        foreach (var device in devices)
                        {
                            var response = _mtconnectAgent.GetDeviceStreamsResponseDocument(device.Uuid, _lastSequence, -1, 1000); // COUNT = 1000 ??
                            if (response != null && response.ObservationCount > 0)
                            {
                                if (SampleReceived != null) SampleReceived.Invoke(device, response);
                                if (response.LastObservationSequence >= lastSequence) lastSequence = response.LastObservationSequence + 1;
                            }
                        }

                        _lastSequence = lastSequence;
                    }

                    await Task.Delay(_configuration.SampleInterval, _stop.Token);
                }
                catch (TaskCanceledException) { }
                catch (Exception ex) { }

            } while (!_stop.Token.IsCancellationRequested);
        }



        private void DeviceAdded(object sender, IDevice device)
        {
            var response = _mtconnectAgent.GetDevicesResponseDocument(device.Uuid);
            if (response != null)
            {
                if (ProbeReceived != null) ProbeReceived.Invoke(device, response);
            }
        }

        private void AssetAdded(object sender, IAsset asset)
        {
            var response = _mtconnectAgent.GetAssetsResponseDocument(new string[] { asset.Uuid });
            if (response != null)
            {
                if (!response.Assets.IsNullOrEmpty())
                {
                    foreach (var responseAsset in response.Assets)
                    {
                        var device = _mtconnectAgent.GetDevice(responseAsset.DeviceUuid);
                        if (device != null)
                        {
                            if (AssetReceived != null) AssetReceived.Invoke(device, response);
                        }
                    }
                }
            }
        }
    }
}
