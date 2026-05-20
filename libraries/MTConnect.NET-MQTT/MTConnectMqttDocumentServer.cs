// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
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
    /// <summary>
    /// Publishes complete MTConnect response documents (Probe, Current, Sample, Asset) over MQTT
    /// on behalf of an in-process agent. The server emits a Probe per device at start, then runs
    /// a worker loop that publishes Current documents on the configured cadence and incremental
    /// Sample documents based on the last observation sequence. Asset documents are published
    /// whenever the agent raises an asset-added event. Consumers subscribe to the topic tree
    /// described by <see cref="IMTConnectMqttDocumentServerConfiguration"/> and the document-type
    /// constants on this class.
    /// </summary>
    public class MTConnectMqttDocumentServer
    {
        /// <summary>Default leaf segment for Probe response publishes; combined with the configuration's topic prefix.</summary>
        public const string ProbeTopic = "Probe";

        /// <summary>Default leaf segment for Current response publishes; combined with the configuration's topic prefix.</summary>
        public const string CurrentTopic = "Current";

        /// <summary>Default leaf segment for Sample response publishes; combined with the configuration's topic prefix.</summary>
        public const string SampleTopic = "Sample";

        /// <summary>Default leaf segment for Asset response publishes; combined with the configuration's topic prefix.</summary>
        public const string AssetTopic = "Asset";

        private readonly IMTConnectAgentBroker _mtconnectAgent;
        private readonly IMTConnectMqttDocumentServerConfiguration _configuration;
        private CancellationTokenSource _stop;
        private ulong _lastSequence;
        private bool _sampleStarted = false;


        /// <summary>Raised whenever the server prepares a Probe document for a device; subscribers serialise and publish the document to the broker.</summary>
        public event MTConnectMqttResponseHandler<IDevicesResponseDocument> ProbeReceived;

        /// <summary>Raised on each tick of the current-publish loop, once per device with the most recent <c>MTConnectStreams</c> snapshot.</summary>
        public event MTConnectMqttResponseHandler<Streams.Output.IStreamsResponseOutputDocument> CurrentReceived;

        /// <summary>Raised whenever new observations beyond the last published sequence become available; carries the incremental <c>MTConnectStreams</c> document.</summary>
        public event MTConnectMqttResponseHandler<Streams.Output.IStreamsResponseOutputDocument> SampleReceived;

        /// <summary>Raised whenever the agent reports a new or updated asset; carries the asset response document for downstream publishing.</summary>
        public event MTConnectMqttResponseHandler<IAssetsResponseDocument> AssetReceived;


        /// <summary>
        /// Hooks into the agent's device/asset add events and prepares the worker state. If
        /// <paramref name="configuration"/> is null a fresh
        /// <see cref="MTConnectMqttDocumentServerConfiguration"/> with its built-in defaults is
        /// used.
        /// </summary>
        /// <param name="mtconnectAgent">The agent broker to read response documents from.</param>
        /// <param name="configuration">Publish cadence and topic-layout settings.</param>
        public MTConnectMqttDocumentServer(IMTConnectAgentBroker mtconnectAgent, IMTConnectMqttDocumentServerConfiguration configuration)
        {
            _mtconnectAgent = mtconnectAgent;
            _mtconnectAgent.DeviceAdded += DeviceAdded;
            _mtconnectAgent.AssetAdded += AssetAdded;

            _configuration = configuration;
            if (_configuration == null) _configuration = new MTConnectMqttDocumentServerConfiguration();
        }


        /// <summary>
        /// Emits one initial Probe document per device by raising <see cref="ProbeReceived"/>,
        /// then launches the background worker that publishes Current documents on the
        /// configured <see cref="IMTConnectMqttDocumentServerConfiguration.CurrentInterval"/>.
        /// </summary>
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
        }

        /// <summary>Cancels the worker tasks. The agent's event subscriptions remain wired; call this before disposing the server.</summary>
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
                                if (_lastSequence < 1) _lastSequence = response.LastObservationSequence;

                                if (CurrentReceived != null) CurrentReceived.Invoke(device, response);
                            }
                        }
                    }


                    _ = Task.Run(SampleWorker, _stop.Token);

                    await Task.Delay(_configuration.CurrentInterval, _stop.Token);
                }
                catch (TaskCanceledException) { }
                catch (Exception) { }

            } while (!_stop.Token.IsCancellationRequested);
        }

        private async Task SampleWorker()
        {
            if (!_sampleStarted)
            {
                _sampleStarted = true;

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
                                var response = _mtconnectAgent.GetDeviceStreamsResponseDocument(device.Uuid, lastSequence + 1, 0, 1000); // COUNT = 1000 ??
                                if (response != null && response.ObservationCount > 0)
                                {
                                    if (SampleReceived != null) SampleReceived.Invoke(device, response);
                                    if (response.LastObservationSequence >= lastSequence) lastSequence = response.LastObservationSequence;
                                }
                            }

                            _lastSequence = lastSequence;
                        }

                        await Task.Delay(_configuration.SampleInterval, _stop.Token);
                    }
                    catch (TaskCanceledException) { }
                    catch (Exception) { }

                } while (!_stop.Token.IsCancellationRequested);
            }
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
            var response = _mtconnectAgent.GetAssetsResponseDocument(new string[] { asset.AssetId });
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
