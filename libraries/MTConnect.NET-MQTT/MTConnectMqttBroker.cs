// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.Extensions.Hosting;
using MQTTnet;
using MQTTnet.Server;
using MTConnect.Agents;
using MTConnect.Assets;
using MTConnect.Devices;
using MTConnect.Observations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Mqtt
{
    /// <summary>
    /// Bridges an in-process <see cref="IMTConnectAgent"/> to an MQTTnet broker, publishing every
    /// device addition, observation, and asset received by the agent as MQTT messages under the
    /// configured topic prefix. Hosts a heartbeat timer that emits the agent's
    /// <c>HeartbeatTimestamp</c> topic on a fixed interval and, optionally, per-interval
    /// observation buckets so consumers can subscribe to coarse-grained snapshots without
    /// processing every individual publish.
    /// </summary>
    public class MTConnectMqttBroker : IHostedService
    {
        private const int _retryInterval = 5000;
        private const string _documentFormat = "JSON";

        private readonly IMTConnectAgent _mtconnectAgent;
        private readonly MqttServer _mqttServer;
        private readonly object _lock = new object();
        private CancellationTokenSource _stop;

        private readonly List<int> _observationIntervals = new List<int>();
        private readonly List<System.Timers.Timer> _observationIntervalTimers = new List<System.Timers.Timer>();
        private readonly Dictionary<int, Dictionary<string, IObservation>> _observationBuffers = new Dictionary<int, Dictionary<string, IObservation>>();

        private readonly int _heartbeatInterval;
        private readonly System.Timers.Timer _heartbeatTimer = new System.Timers.Timer();


        /// <summary>Heartbeat interval in milliseconds; the broker emits the agent's <c>HeartbeatTimestamp</c> publish at this cadence.</summary>
        public int HeartbeatInterval => _heartbeatInterval;

        /// <summary>Topic layout (flat or hierarchy) used for observation publishes; defaults to <see cref="MTConnectMqttFormat.Hierarchy"/>.</summary>
        public MTConnectMqttFormat Format { get; set; }

        /// <summary>Optional topic prefix used in place of the default <c>MTConnect</c>; when null the standard prefix is applied by <see cref="MTConnectMqttMessage"/>.</summary>
        public string TopicPrefix { get; set; }

        /// <summary>When true, retained publishes are used so newly connecting clients see the latest agent/device/observation state immediately.</summary>
        public bool RetainMessages { get; set; }

        /// <summary>Raised by the underlying <see cref="MqttServer"/> when a downstream MQTT client connects.</summary>
        public event EventHandler ClientConnected;

        /// <summary>Raised by the underlying <see cref="MqttServer"/> when a downstream MQTT client disconnects.</summary>
        public event EventHandler ClientDisconnected;

        #pragma warning disable CS0067 // event is part of the public API surface, raised by subclasses
        /// <summary>Raised after each successful publish; the argument is the topic name.</summary>
        public event EventHandler<string> MessageSent;
        #pragma warning restore CS0067

        /// <summary>Raised when the broker's connection to its underlying transport fails.</summary>
        public event EventHandler<Exception> ConnectionError;

        #pragma warning disable CS0067 // event is part of the public API surface, raised by subclasses
        /// <summary>Raised when a publish fails after the connection has been established.</summary>
        public event EventHandler<Exception> PublishError;
        #pragma warning restore CS0067


        /// <summary>
        /// Subscribes to the agent's add events and prepares the broker for hosting. The broker
        /// is not started until <see cref="StartAsync"/> is called by the .NET hosting layer.
        /// </summary>
        /// <param name="mtconnectAgent">The agent to mirror over MQTT; its add events are observed.</param>
        /// <param name="mqttServer">The MQTTnet server instance the broker publishes through; ownership stays with the caller.</param>
        /// <param name="observationIntervals">Optional list of interval-bucket lengths in milliseconds; if supplied, the broker emits batched observation topics for each interval.</param>
        /// <param name="heartbeatInterval">Heartbeat publish cadence in milliseconds; defaults to one second.</param>
        public MTConnectMqttBroker(IMTConnectAgent mtconnectAgent, MqttServer mqttServer, IEnumerable<int> observationIntervals = null, int heartbeatInterval = 1000)
        {
            _mtconnectAgent = mtconnectAgent;
            _mtconnectAgent.DeviceAdded += DeviceAdded;
            _mtconnectAgent.ObservationAdded += ObservationAdded;
            _mtconnectAgent.AssetAdded += AssetAdded;

            Format = MTConnectMqttFormat.Hierarchy;
            RetainMessages = true;

            _mqttServer = mqttServer;
            _mqttServer.ClientConnectedAsync += async (args) =>
            {
                await Task.CompletedTask;
                if (ClientConnected != null) ClientConnected.Invoke(this, new EventArgs());
            };
            _mqttServer.ClientDisconnectedAsync += async (args) =>
            {
                await Task.CompletedTask;
                if (ClientDisconnected != null) ClientDisconnected.Invoke(this, new EventArgs());
            };

            // Set Observation Intervals
            if (!observationIntervals.IsNullOrEmpty())
            {
                _observationIntervals.AddRange(observationIntervals);
            }

            // Set Heartbeat Timer
            _heartbeatInterval = heartbeatInterval;
            _heartbeatTimer.Interval = _heartbeatInterval;
            _heartbeatTimer.Elapsed += HeartbeatTimerElapsed;
        }


        /// <summary>
        /// <see cref="IHostedService"/> entry point. Spawns the broker worker on the thread pool,
        /// starts the heartbeat timer, and starts one timer per configured observation interval
        /// that flushes buffered observations under the interval-bucketed topic on each tick.
        /// </summary>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            _stop = new CancellationTokenSource();

            if (!_mqttServer.IsStarted)
            {
                _ = Task.Run(Worker, _stop.Token);

                _heartbeatTimer.Start();

                if (!_observationIntervals.IsNullOrEmpty())
                {
                    var timerIntervals = _observationIntervals.Where(o => o > 0);
                    if (!timerIntervals.IsNullOrEmpty())
                    {
                        foreach (var interval in timerIntervals)
                        {
                            var timer = new System.Timers.Timer();
                            timer.Interval = interval;
                            timer.Elapsed += ObservationIntervalTimerElapsed;
                            lock (_lock) _observationIntervalTimers.Add(timer);

                            timer.Start();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// <see cref="IHostedService"/> shutdown hook. Cancels the worker, stops the heartbeat
        /// timer, disposes the interval-bucket timers, and awaits a graceful shutdown of the
        /// underlying <see cref="MqttServer"/>.
        /// </summary>
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_stop != null) _stop.Cancel();
            if (_heartbeatTimer != null) _heartbeatTimer.Stop();

            if (!_observationIntervalTimers.IsNullOrEmpty())
            {
                foreach (var timer in _observationIntervalTimers) timer.Dispose();
                _observationIntervalTimers.Clear();
            }

            if (_mqttServer != null) await _mqttServer.StopAsync();
        }


        private async Task Worker()
        {
            do
            {
                try
                {
                    try
                    {
                        await _mqttServer.StartAsync();

                        // Publish MTConnect Agent Information
                        await PublishAgent(_mtconnectAgent);

                        var devices = _mtconnectAgent.GetDevices();
                        foreach (var device in devices)
                        {
                            // Publish the Device
                            await PublishDevice(device);
                        }

                        // Add Current Observations (to Initialize each DataItem on the MQTT broker)
                        var observations = _mtconnectAgent.GetCurrentObservations();
                        if (!observations.IsNullOrEmpty())
                        {
                            foreach (var observationOutput in observations)
                            {
                                var observation = Observation.Create(observationOutput.DataItem);
                                observation.DeviceUuid = observationOutput.DeviceUuid;
                                observation.DataItem = observationOutput.DataItem;
                                observation.InstanceId = observationOutput.InstanceId;
                                observation.Timestamp = observationOutput.Timestamp;
                                observation.AddValues(observationOutput.Values);

                                await PublishObservation(observation);
                            }
                        }

                        while (!_stop.Token.IsCancellationRequested)
                        {
                            await Task.Delay(100);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ConnectionError != null) ConnectionError.Invoke(this, ex);
                    }

                    await Task.Delay(_retryInterval, _stop.Token);
                }
                catch (TaskCanceledException) { }
                catch (Exception) { }

            } while (!_stop.Token.IsCancellationRequested);
        }


        private async void DeviceAdded(object sender, IDevice device)
        {
            await PublishDevice(device);
        }

        private async void ObservationAdded(object sender, IObservation observation)
        {
            await PublishObservation(observation);
        }

        private async void AssetAdded(object sender, IAsset asset)
        {
            await PublishAsset(asset);
        }


        private async Task PublishAgent(IMTConnectAgent agent)
        {
            var messages = MTConnectMqttMessage.Create(agent, _observationIntervals, _heartbeatInterval, RetainMessages);
            if (!messages.IsNullOrEmpty())
            {
                foreach (var message in messages)
                {
                    if (message != null && message.HasPayload())
                    {
                        await Publish(message);
                    }
                }
            }
        }

        private async Task PublishDevice(IDevice device)
        {
            var messages = MTConnectMqttMessage.Create(device, _mtconnectAgent.Uuid, _documentFormat, RetainMessages);
            if (!messages.IsNullOrEmpty())
            {
                foreach (var message in messages)
                {
                    if (message != null && message.HasPayload())
                    {
                        await Publish(message);
                    }
                }
            }
        }

        private async Task PublishObservation(IObservation observation)
        {
            if (!_observationIntervals.IsNullOrEmpty())
            {
                foreach (var interval in _observationIntervals)
                {
                    if (interval > 0)
                    {
                        var bufferKey = CreateBufferKey(observation.DeviceUuid, observation.DataItemId, interval);
                        if (!string.IsNullOrEmpty(bufferKey))
                        {
                            lock (_lock)
                            {
                                _observationBuffers.TryGetValue(interval, out var buffer);
                                if (buffer == null)
                                {
                                    buffer = new Dictionary<string, IObservation>();
                                    _observationBuffers.Add(interval, buffer);
                                }

                                buffer.Remove(bufferKey);
                                buffer.Add(bufferKey, observation);
                            }
                        }
                    }
                    else
                    {
                        await PublishObservation(observation, 0);
                    }
                }
            }
            else
            {
                await PublishObservation(observation, 0);
            }
        }

        private async Task PublishObservation(IObservation observation, int interval)
        {
            if (observation.Category != Devices.DataItemCategory.CONDITION)
            {
                var message = MTConnectMqttMessage.Create(observation, Format, _documentFormat, RetainMessages, interval);
                if (message != null && message.HasPayload()) await Publish(message);
            }
            else
            {
                var observations = _mtconnectAgent.GetCurrentObservations(observation.DeviceUuid);
                if (!observations.IsNullOrEmpty())
                {
                    var dataItemObservations = observations.Where(o => o.DataItemId == observation.DataItemId);
                    if (!dataItemObservations.IsNullOrEmpty())
                    {
                        var x = new List<IObservation>();
                        foreach (var dataItemObservation in dataItemObservations)
                        {
                            var y = Observation.Create(dataItemObservation.DataItem);
                            y.DeviceUuid = dataItemObservation.DeviceUuid;
                            y.DataItem = dataItemObservation.DataItem;
                            y.InstanceId = dataItemObservation.InstanceId;
                            y.Timestamp = dataItemObservation.Timestamp;
                            y.AddValues(dataItemObservation.Values);
                            x.Add(y);
                        }

                        var message = MTConnectMqttMessage.Create(x, Format, _documentFormat, RetainMessages, interval);
                        if (message != null && message.HasPayload()) await Publish(message);
                    }
                }
            }
        }

        private async Task PublishAsset(IAsset asset)
        {
            var messages = MTConnectMqttMessage.Create(asset, _documentFormat, RetainMessages);
            await Publish(messages);
        }


        private async Task Publish(MqttApplicationMessage message)
        {
            if (_mqttServer != null && _mqttServer.IsStarted)
            {
                // Set the Topic Prefix
                if (!string.IsNullOrEmpty(TopicPrefix)) message.Topic = $"{TopicPrefix}/{message.Topic}";

                var injectMessage = new InjectedMqttApplicationMessage(message);

                await _mqttServer.InjectApplicationMessage(injectMessage);
            }
        }

        private async Task Publish(IEnumerable<MqttApplicationMessage> messages)
        {
            if (_mqttServer != null && _mqttServer.IsStarted && !messages.IsNullOrEmpty())
            {
                foreach (var message in messages)
                {
                    // Set the Topic Prefix
                    if (!string.IsNullOrEmpty(TopicPrefix)) message.Topic = $"{TopicPrefix}/{message.Topic}";

                    await _mqttServer.InjectApplicationMessage(new InjectedMqttApplicationMessage(message));
                }
            }
        }


        private async void HeartbeatTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            await Publish(MTConnectMqttMessage.CreateHeartbeat(_mtconnectAgent, UnixDateTime.Now));
        }

        private async void ObservationIntervalTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (sender != null)
            {
                var timer = (System.Timers.Timer)sender;
                var interval = (int)timer.Interval;

                Dictionary<string, IObservation> buffer;
                lock (_lock)
                {
                    _observationBuffers.TryGetValue(interval, out buffer);
                    _observationBuffers.Remove(interval);
                }

                if (!buffer.IsNullOrEmpty())
                {
                    foreach (var observation in buffer.Values)
                    {
                        await PublishObservation(observation, interval);
                    }
                }
            }
        }


        private static string CreateBufferKey(string deviceUuid, string dataItemId, int interval)
        {
            if (!string.IsNullOrEmpty(deviceUuid) && !string.IsNullOrEmpty(dataItemId) && interval > 0)
            {
                return $"{deviceUuid}::{dataItemId}::{interval}";
            }

            return null;
        }
    }
}