// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MQTTnet;
using MQTTnet.Client;
using MTConnect.Agents;
using MTConnect.Assets;
using MTConnect.Clients;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Formatters;
using MTConnect.Logging;
using MTConnect.Observations;
using MTConnect.Observations.Events;
using MTConnect.Observations.Output;
using MTConnect.Streams.Output;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect
{
    public class Module : MTConnectAgentModule
    {
        public const string ConfigurationTypeId = "mqtt-relay";
        private const string ModuleId = "MQTT Relay";

        private readonly MqttRelayModuleConfiguration _configuration;
        private readonly MTConnectMqttDocumentServer _documentServer;
        private readonly MTConnectMqttEntityServer _entityServer;
        private readonly MqttFactory _mqttFactory;
        private readonly IMqttClient _mqttClient;
        private CancellationTokenSource _stop;
        private static readonly object _lastSentSequenceLock = new object();
        private long _totalIncomingObservations = 0;
        // In-memory persister: tracks the last-sent sequence under
        // DurableRelay and flushes to disk on a timer, on shutdown,
        // and at each buffered-relay batch boundary, so the observation
        // hot path stays decoupled from disk IO.
        private readonly LastSentSequencePersister _lastSentSequencePersister = new LastSentSequencePersister();
        private System.Threading.Timer _lastSentFlushTimer;
        private static readonly TimeSpan LastSentFlushInterval = TimeSpan.FromSeconds(1);
        // Count of in-flight AsyncVoidGuard.Run invocations on the
        // ThreadPool. OnStop polls this against zero so the final
        // last-sent-sequence flush captures every RecordLastSentSequence
        // call that was already underway when the cancel token fired.
        private int _handlerActiveCount = 0;
        private static readonly TimeSpan HandlerDrainTimeout = TimeSpan.FromMilliseconds(500);
        private static readonly TimeSpan HandlerDrainPollInterval = TimeSpan.FromMilliseconds(10);
        private const string DirectoryBuffer = "buffer";
        private const string LastSentSequenceFileName = "mqttrelay_last_sent.seq";

        public Module(IMTConnectAgentBroker mtconnectAgent, object configuration) : base(mtconnectAgent)
        {
            Id = ModuleId;

            _configuration = AgentApplicationConfiguration.GetConfiguration<MqttRelayModuleConfiguration>(configuration);

            switch (_configuration.TopicStructure)
            {
                case MqttTopicStructure.Document:

                    _documentServer = new MTConnectMqttDocumentServer(mtconnectAgent, _configuration);
                    _documentServer.ProbeReceived += ProbeReceived;
                    _documentServer.CurrentReceived += CurrentReceived;
                    _documentServer.SampleReceived += SampleReceived;
                    _documentServer.AssetReceived += AssetReceived;
                    break;


                case MqttTopicStructure.Entity:

                    _entityServer = new MTConnectMqttEntityServer(_configuration.TopicPrefix, _configuration.DocumentFormat, _configuration.Qos);
                    Agent.DeviceAdded += AgentDeviceAdded;
                    Agent.ObservationAdded += AgentObservationAdded;
                    Agent.AssetAdded += AgentAssetAdded;
                    break;
            }

            _mqttFactory = new MqttFactory();
            _mqttClient = _mqttFactory.CreateMqttClient();
        }


        protected override void OnStartAfterLoad(bool initializeDataItems)
        {
            _stop = new CancellationTokenSource();

            // Seed the in-memory persister from disk so the first
            // RelayBufferedObservations diagnostic uses the durable
            // value rather than zero.
            if (_configuration.DurableRelay)
            {
                _lastSentSequencePersister.Initialize(ReadLastSentSequenceFromDisk());

                // Start a background flush timer. The timer only emits
                // a write when the persister is dirty so an idle relay
                // does not burn IOPS.
                _lastSentFlushTimer = new System.Threading.Timer(
                    _ => FlushLastSentSequence(),
                    null,
                    LastSentFlushInterval,
                    LastSentFlushInterval);
            }

            _ = Task.Run(Worker, _stop.Token);
        }

        protected override void OnStop()
        {
            // Entity-mode constructs only _entityServer, so a bare
            // _documentServer.Stop() here would raise an NRE during
            // shutdown. Route through the lifecycle helper so each
            // server stop is independently null-safe and exception-safe.
            MqttRelayLifecycle.StopServers(
                documentStop: _documentServer != null ? (Action)(() => _documentServer.Stop()) : null,
                entityStop: null);

            // Cancel first so observation event handlers see the
            // cancellation signal and the Worker exits its
            // reconnect-delay branch promptly. The flush ordering below
            // depends on cancelling BEFORE the final flush so a
            // RecordLastSentSequence call that is mid-await cannot land
            // an update after the flush has already drained the dirty
            // bit.
            if (_stop != null) _stop.Cancel();

            // Wait briefly for in-flight AsyncVoidGuard.Run handlers to
            // drain. The counter is incremented at the top of each
            // handler body (AgentObservationAdded is the relevant one:
            // it calls RecordLastSentSequence) and decremented after the
            // body completes. A bounded poll keeps shutdown forward-
            // progress: a handler hung on a broker call cannot block
            // the agent from terminating.
            var drainDeadline = DateTime.UtcNow + HandlerDrainTimeout;
            while (Interlocked.CompareExchange(ref _handlerActiveCount, 0, 0) > 0
                && DateTime.UtcNow < drainDeadline)
            {
                System.Threading.Thread.Sleep(HandlerDrainPollInterval);
            }

            // Dispose the flush timer AFTER the handlers have drained so
            // any final timer tick that fires concurrently with the
            // drain still serialises on _lastSentSequenceLock.
            if (_lastSentFlushTimer != null)
            {
                try { _lastSentFlushTimer.Dispose(); } catch { }
                _lastSentFlushTimer = null;
            }

            // Final flush captures every RecordLastSentSequence call
            // that landed before the drain window elapsed; a clean
            // shutdown therefore does not lose progress against the
            // buffered-observation relay.
            FlushLastSentSequence();

            // Bounded await on the disconnect. The MqttRelayLifecycle
            // helper surfaces faults to the log and a hung broker
            // cannot block shutdown indefinitely.
            MqttRelayLifecycle.DisconnectWithTimeout(
                disconnect: _mqttClient != null
                    ? (Func<Task>)(() => _mqttClient.DisconnectAsync(MqttClientDisconnectOptionsReason.NormalDisconnection))
                    : null,
                timeout: TimeSpan.FromSeconds(5),
                onFault: ex => Log(MTConnectLogLevel.Warning, $"MQTT Relay Disconnect Error : {ex.Message}"));
        }

        private async Task Worker()
        {
            do
            {
                try
                {
                    try
                    {
                        // Declare new MQTT Client Options with Tcp Server
                        var clientOptionsBuilder = new MqttClientOptionsBuilder();

                        // Add TCP Server
                        clientOptionsBuilder.WithTcpServer(_configuration.Server, _configuration.Port);

                        // Publish Only so use Clean Session = true
                        clientOptionsBuilder.WithCleanSession();

                        // Sets the Timeout
                        clientOptionsBuilder.WithTimeout(TimeSpan.FromMilliseconds(_configuration.Timeout));

                        // Set LWT (Agent Available). AvailabilityTopic.Build
                        // returns null when TopicPrefix or Agent.Uuid is
                        // missing or contains an MQTT-reserved character;
                        // passing null into WithWillTopic raises in MQTTnet's
                        // ClientOptionsBuilder.Build(), so a misconfigured
                        // topic must skip the will-topic registration and
                        // surface a fatal-config warning instead.
                        var willTopic = GetAgentAvailableTopic();
                        if (!string.IsNullOrEmpty(willTopic))
                        {
                            clientOptionsBuilder.WithWillTopic(willTopic);
                            clientOptionsBuilder.WithWillPayload(System.Text.Encoding.UTF8.GetBytes(Availability.UNAVAILABLE.ToString()));
                            clientOptionsBuilder.WithWillQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce);
                            clientOptionsBuilder.WithWillRetain(true);
                        }
                        else
                        {
                            Log(MTConnectLogLevel.Fatal, $"MQTT Relay Configuration Error : Agent Available topic could not be built from TopicPrefix='{_configuration?.TopicPrefix}' and AgentUuid='{Agent?.Uuid}'; the broker will not receive a Last Will and Testament for this agent.");
                        }

                        // Set Client ID
                        if (!string.IsNullOrEmpty(_configuration.ClientId))
                        {
                            clientOptionsBuilder.WithClientId(_configuration.ClientId);
                        }

                        // Set TLS Certificate
                        if (_configuration.Tls != null)
                        {
                            var certificateResults = _configuration.Tls.GetCertificate();
                            if (certificateResults.Success && certificateResults.Certificate != null)
                            {
                                var certificateAuthorityResults = _configuration.Tls.GetCertificateAuthority();

                                var certificates = new List<X509Certificate2>();
                                if (certificateAuthorityResults.Certificate != null && _configuration.Tls.OmitCAValidation == false)
                                {
                                    certificates.Add(certificateAuthorityResults.Certificate);
                                }
                                certificates.Add(certificateResults.Certificate);

                                var tlsOptionsBuilder = new MqttClientTlsOptionsBuilder();

                                // Set Client Certificate
                                tlsOptionsBuilder.WithClientCertificates(certificates);

                                // Set VerifyClientCertificate option
                                tlsOptionsBuilder.WithAllowUntrustedCertificates(!_configuration.Tls.VerifyClientCertificate);

#if NET5_0_OR_GREATER
                                // Setup CA Certificate
                                if (certificateAuthorityResults.Certificate != null)
                                {
                                    tlsOptionsBuilder.WithCertificateValidationHandler((certContext) =>
                                    {
                                        var chain = new X509Chain();
                                        chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                                        chain.ChainPolicy.RevocationFlag = X509RevocationFlag.ExcludeRoot;
                                        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.NoFlag;
                                        chain.ChainPolicy.VerificationTime = DateTime.Now;
                                        chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 0, 0);
                                        chain.ChainPolicy.CustomTrustStore.Add(certificateAuthorityResults.Certificate);
                                        chain.ChainPolicy.TrustMode = X509ChainTrustMode.CustomRootTrust;

                                        // convert provided X509Certificate to X509Certificate2
                                        var x5092 = new X509Certificate2(certContext.Certificate);

                                        return chain.Build(x5092);
                                    });
                                }
#endif

                                clientOptionsBuilder.WithTlsOptions(tlsOptionsBuilder.Build());
                            }
                        }

                        // Add Credentials
                        if (!string.IsNullOrEmpty(_configuration.Username) && !string.IsNullOrEmpty(_configuration.Password))
                        {
                            if (_configuration.UseTls)
                            {
                                var tlsOptionsBuilder = new MqttClientTlsOptionsBuilder();
                                tlsOptionsBuilder.WithSslProtocols(System.Security.Authentication.SslProtocols.Tls12);
                                clientOptionsBuilder.WithTlsOptions(tlsOptionsBuilder.Build());

                                clientOptionsBuilder.WithCredentials(_configuration.Username, _configuration.Password);
                            }
                            else
                            {
                                clientOptionsBuilder.WithCredentials(_configuration.Username, _configuration.Password);
                            }
                        }

                        // Build MQTT Client Options
                        var clientOptions = clientOptionsBuilder.Build();

                        await _mqttClient.ConnectAsync(clientOptions, CancellationToken.None);

                        Log(MTConnectLogLevel.Information, $"MQTT Relay Connected to External Broker ({_configuration.Server}:{_configuration.Port})");

                        // Start Document Server (if configured)
                        if (_documentServer != null) _documentServer.Start();

                        // Initialize Entity Server (if configured)
                        if (_entityServer != null)
                        {
                            if (_configuration.DurableRelay)
                            {
                                await RelayBufferedObservations();
                            }
                            await PublishDevices();
                            await PublishCurrentObservations();
                            await PublishAssets();
                        }

                        while (!_stop.Token.IsCancellationRequested && _mqttClient.IsConnected)
                        {
                            await Task.Delay(100);
                        }
                    }
                    catch (Exception exception)
                    {
                        Log(MTConnectLogLevel.Warning, $"MQTT Relay Connection Error : {exception.Message}");

                        var innerException = exception.InnerException;

                        while (innerException != null)
                        {
                            Log(MTConnectLogLevel.Warning, $"MQTT Relay Connection Error (InnerException) : {innerException.Message}");
                            innerException = innerException.InnerException;
                        }

                        Log(MTConnectLogLevel.Warning, $"MQTT Relay Connection Error (BaseException) : {exception.GetBaseException().ToString()}");
                    }
                    finally
                    {
                        if (_documentServer != null) _documentServer.Stop();
                    }

                    Log(MTConnectLogLevel.Information, $"MQTT Relay Disconnected from External Broker ({_configuration.Server}:{_configuration.Port})");

                    await Task.Delay(_configuration.ReconnectInterval, _stop.Token);
                }
                catch (TaskCanceledException) { }
                catch (Exception ex)
                {
                    // Route through WorkerLoopExceptionLogger so the
                    // operator sees an unexpected defect at the outer
                    // scope instead of having it silently swallowed.
                    WorkerLoopExceptionLogger.Log(
                        exception: ex,
                        onLog: msg => Log(MTConnectLogLevel.Warning, msg));
                }

            } while (!_stop.Token.IsCancellationRequested);
        }

        private static IEnumerable<IObservationOutput> GetAllObservations(IStreamsResponseOutputDocument doc)
        {
            if (doc == null || doc.Streams == null) yield break;

            foreach (var deviceStream in doc.Streams)
            {
                if (deviceStream.ComponentStreams == null) continue;
                foreach (var componentStream in deviceStream.ComponentStreams)
                {
                    if (componentStream.Observations == null) continue;
                    foreach (var obs in componentStream.Observations)
                    {
                        yield return obs;
                    }
                }
            }
        }

        private async Task RelayBufferedObservations()
        {
            if (Agent is IMTConnectAgentBroker broker)
            {
                // Read in-memory: the persister was seeded from disk at
                // OnStartAfterLoad. Avoid a synchronous file read on
                // every relay attempt.
                ulong lastSent = _lastSentSequencePersister.Read();
                ulong from = Math.Max(lastSent + 1, broker.FirstSequence);
                ulong to = broker.LastSequence;

                Log(MTConnectLogLevel.Information, $"[MQTT Relay] RelayBufferedObservations: lastSent={lastSent}, broker.FirstSequence={broker.FirstSequence}, broker.LastSequence={broker.LastSequence}");

                long missed = RelayBufferDiagnostics.ComputeMissed(to, lastSent);

                if (lastSent + 1 < broker.FirstSequence)
                {
                    Log(MTConnectLogLevel.Warning, $"[MQTT Relay] Some observations could not be relayed because they were overwritten in the buffer (lastSent={lastSent}, firstAvailable={broker.FirstSequence}).");
                }
                if (missed > 0 && from <= to)
                {
                    Log(MTConnectLogLevel.Warning, $"[MQTT Relay] Network reconnected. {to - from + 1} observations will now be sent (from Sequence {from} to {to}).");
                }

                if (from <= to)
                {
                    var doc = broker.GetDeviceStreamsResponseDocument(from, to);
                    var observations = GetAllObservations(doc)
                        .OrderBy(o => o.Sequence)
                        .ToList();

                    Log(MTConnectLogLevel.Information, $"[MQTT Relay] Relaying buffered observations: {observations.Count} from Sequence {from} to {to}");

                    foreach (var obs in observations)
                    {
                        var x = new Observation();
                        x.DeviceUuid = obs.DeviceUuid;
                        x.DataItemId = obs.DataItemId;
                        x.DataItem = obs.DataItem;
                        x.Name = obs.Name;
                        x.Category = obs.Category;
                        x.Type = obs.Type;
                        x.SubType = obs.SubType;
                        x.Representation = obs.Representation;
                        x.CompositionId = obs.CompositionId;
                        x.InstanceId = obs.InstanceId;
                        x.Sequence = obs.Sequence;
                        x.Timestamp = obs.Timestamp;
                        x.AddValues(obs.Values);

                        var result = await _entityServer.PublishObservation(_mqttClient, x);
                        if (result != null && result.IsSuccess)
                        {
                            RecordLastSentSequence(x.Sequence);
                        }
                    }

                    // Batch boundary: the buffered relay just ran a
                    // chunk of observations under DurableRelay; flush
                    // the persister so a crash before the next timer
                    // tick does not lose this batch's progress.
                    FlushLastSentSequence();

                    Log(MTConnectLogLevel.Information, $"[MQTT Relay] Buffered observation relay complete. {observations.Count} missed observations sent.");
                    var lastSentSnapshot = _lastSentSequencePersister.Read();
                    var unsent = broker.LastSequence > lastSentSnapshot ? broker.LastSequence - lastSentSnapshot : 0;
                    if (unsent > 0)
                        Log(MTConnectLogLevel.Information, $"[MQTT Relay] {unsent} new observations arrived during relay and will be sent next.");
                }
                else
                {
                    Log(MTConnectLogLevel.Information, "[MQTT Relay] No buffered observations to relay.");
                }
            }
        }

        private static string GetLastSentSequenceFilePath(IAgentApplicationConfiguration agentConfig = null)
        {
            string baseDir = !string.IsNullOrEmpty(agentConfig?.DurableBufferPath)
                ? agentConfig.DurableBufferPath
                : DirectoryBuffer;

            if (!Path.IsPathRooted(baseDir))
            {
                baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, baseDir);
            }
            Directory.CreateDirectory(baseDir);
            return Path.Combine(baseDir, LastSentSequenceFileName);
        }

        // Disk read used at startup (Initialize) only. Hot-path reads
        // go through _lastSentSequencePersister.Read(); this method
        // is the single point of entry for the actual file IO.
        private ulong ReadLastSentSequenceFromDisk()
        {
            if (!_configuration.DurableRelay) return 0;
            var agentConfig = Agent?.Configuration as IAgentApplicationConfiguration;
            var path = GetLastSentSequenceFilePath(agentConfig);
            lock (_lastSentSequenceLock)
            {
                if (File.Exists(path))
                {
                    var text = File.ReadAllText(path);
                    if (ulong.TryParse(text, out var seq)) return seq;
                }
            }
            return 0;
        }

        // Records a new in-memory last-sent sequence. The actual disk
        // write happens out-of-band on the flush timer / shutdown /
        // batch boundary via FlushLastSentSequence.
        private void RecordLastSentSequence(ulong seq)
        {
            if (!_configuration.DurableRelay) return; // Default
            _lastSentSequencePersister.Update(seq);
        }

        // Writes the in-memory persister value to disk if dirty. Safe
        // to call from any thread; the inner File.WriteAllText runs
        // under _lastSentSequenceLock so a timer tick cannot race a
        // shutdown flush.
        private void FlushLastSentSequence()
        {
            if (!_configuration.DurableRelay) return;

            try
            {
                var agentConfig = Agent?.Configuration as IAgentApplicationConfiguration;
                var path = GetLastSentSequenceFilePath(agentConfig);

                _lastSentSequencePersister.TryFlush(seq =>
                {
                    lock (_lastSentSequenceLock)
                    {
                        File.WriteAllText(path, seq.ToString());
                    }
                });
            }
            catch (Exception ex)
            {
                Log(MTConnectLogLevel.Warning, $"MQTT Relay last-sent-sequence flush error : {ex.Message}");
            }
        }

        private async void ProbeReceived(IDevice device, IDevicesResponseDocument responseDocument)
        {
            // async void handler: route any unhandled fault (formatter
            // throw, message-builder failure) to the log instead of the
            // synchronization context.
            await AsyncVoidGuard.Run(
                async () => await ProbeReceivedCore(device, responseDocument),
                ex => Log(MTConnectLogLevel.Warning, $"ProbeReceived handler error : {ex.Message}"));
        }

        private async Task ProbeReceivedCore(IDevice device, IDevicesResponseDocument responseDocument)
        {
            if (_mqttClient != null && _mqttClient.IsConnected)
            {
                var x = new List<KeyValuePair<string, string>>();
                x.Add(new KeyValuePair<string, string>("indentOutput", _configuration.IndentOutput.ToString()));

                var formatResult = ResponseDocumentFormatter.Format(_configuration.DocumentFormat, responseDocument, x);
                if (formatResult.Success)
                {
                    var topic = $"{_configuration.TopicPrefix}/{MTConnectMqttDocumentServer.ProbeTopic}/{device.Uuid}";
                    if (formatResult.Content != null && formatResult.Content.Position > 0) formatResult.Content.Seek(0, SeekOrigin.Begin);

                    try
                    {
                        var messageBuilder = new MqttApplicationMessageBuilder();
                        messageBuilder.WithRetainFlag(true);
                        messageBuilder.WithTopic(topic);
                        messageBuilder.WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce);
                        messageBuilder.WithPayload(formatResult.Content);
                        var message = messageBuilder.Build();

                        var publishResult = await _mqttClient.PublishAsync(message);
                        if (publishResult.IsSuccess)
                        {
                            Log(MTConnectLogLevel.Debug, $"Probe : Published to Topic ({topic})");
                        }
                        else
                        {
                            Log(MTConnectLogLevel.Warning, $"Probe : Error Publishing to Topic ({topic}) : {publishResult.ReasonCode} : {publishResult.ReasonString}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Log(MTConnectLogLevel.Warning, $"Probe : Error Publishing to Topic ({topic}) : {ex.Message}");
                    }


                    // Write Available (for Agent Device)
                    if (device.Type == Devices.Agent.TypeId)
                    {
                        var availableTopic = GetAgentAvailableTopic();
                        if (string.IsNullOrEmpty(availableTopic))
                        {
                            // AvailabilityTopic.Build rejected the input
                            // (null / empty / wildcard-segment TopicPrefix
                            // or AgentUuid). Skip the availability publish
                            // because passing null into WithTopic raises in
                            // MQTTnet's MessageBuilder.Build(), and a
                            // partially-correct topic risks publishing under
                            // the {TopicPrefix}/Probe/# wildcard the fix
                            // exists to keep JSON-pure.
                            Log(MTConnectLogLevel.Warning, $"Agent Available : Skipped because availability topic could not be built from TopicPrefix='{_configuration?.TopicPrefix}' and AgentUuid='{Agent?.Uuid}'.");
                        }
                        else
                        {
                            var availablePayload = System.Text.Encoding.UTF8.GetBytes(Availability.AVAILABLE.ToString());

                            try
                            {
                                var messageBuilder = new MqttApplicationMessageBuilder();
                                messageBuilder.WithRetainFlag(true);
                                messageBuilder.WithTopic(availableTopic);
                                messageBuilder.WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce);
#if NET5_0_OR_GREATER
                                messageBuilder.WithPayloadSegment(availablePayload);
#else
                                messageBuilder.WithPayload(availablePayload);
#endif
                                var message = messageBuilder.Build();

                                var publishResult = await _mqttClient.PublishAsync(message);
                                if (publishResult.IsSuccess)
                                {
                                    Log(MTConnectLogLevel.Debug, $"Agent Available : Published to Topic ({availableTopic})");
                                }
                                else
                                {
                                    Log(MTConnectLogLevel.Warning, $"Agent Available : Error Publishing to Topic ({availableTopic}) : {publishResult.ReasonCode} : {publishResult.ReasonString}");
                                }
                            }
                            catch (Exception ex)
                            {
                                Log(MTConnectLogLevel.Warning, $"Agent Available : Error Publishing to Topic ({availableTopic}) : {ex.Message}");
                            }
                        }
                    }
                }
            }
        }

        private async void CurrentReceived(IDevice device, IStreamsResponseOutputDocument responseDocument)
        {
            // async void handler: route any unhandled fault to the log
            // instead of the synchronization context.
            await AsyncVoidGuard.Run(
                async () => await CurrentReceivedCore(device, responseDocument),
                ex => Log(MTConnectLogLevel.Warning, $"CurrentReceived handler error : {ex.Message}"));
        }

        private async Task CurrentReceivedCore(IDevice device, IStreamsResponseOutputDocument responseDocument)
        {
            if (_mqttClient != null && _mqttClient.IsConnected)
            {
                var x = new List<KeyValuePair<string, string>>();
                x.Add(new KeyValuePair<string, string>("indentOutput", _configuration.IndentOutput.ToString()));

                var formatResult = ResponseDocumentFormatter.Format(_configuration.DocumentFormat, ref responseDocument, x);
                if (formatResult.Success)
                {
                    var topic = $"{_configuration.TopicPrefix}/{MTConnectMqttDocumentServer.CurrentTopic}/{device.Uuid}";
                    if (formatResult.Content != null && formatResult.Content.Position > 0) formatResult.Content.Seek(0, SeekOrigin.Begin);

                    var messageBuilder = new MqttApplicationMessageBuilder();
                    //messageBuilder.WithRetainFlag(true);
                    messageBuilder.WithTopic(topic);
                    messageBuilder.WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce);
                    messageBuilder.WithPayload(formatResult.Content);
                    var message = messageBuilder.Build();

                    try
                    {
                        var publishResult = await _mqttClient.PublishAsync(message);
                        if (publishResult.IsSuccess)
                        {
                            Log(MTConnectLogLevel.Debug, $"Current : Published to Topic ({topic})");
                        }
                        else
                        {
                            Log(MTConnectLogLevel.Warning, $"Current : Error Publishing to Topic ({topic}) : {publishResult.ReasonCode} : {publishResult.ReasonString}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Log(MTConnectLogLevel.Warning, $"Current : Error Publishing to Topic ({topic}) : {ex.Message}");
                    }
                }
            }
        }

        private async void SampleReceived(IDevice device, IStreamsResponseOutputDocument responseDocument)
        {
            // async void handler: route any unhandled fault to the log
            // instead of the synchronization context.
            await AsyncVoidGuard.Run(
                async () => await SampleReceivedCore(device, responseDocument),
                ex => Log(MTConnectLogLevel.Warning, $"SampleReceived handler error : {ex.Message}"));
        }

        private async Task SampleReceivedCore(IDevice device, IStreamsResponseOutputDocument responseDocument)
        {
            if (_mqttClient != null && _mqttClient.IsConnected)
            {
                var x = new List<KeyValuePair<string, string>>();
                x.Add(new KeyValuePair<string, string>("indentOutput", _configuration.IndentOutput.ToString()));

                var formatResult = ResponseDocumentFormatter.Format(_configuration.DocumentFormat, ref responseDocument, x);
                if (formatResult.Success)
                {
                    var topic = $"{_configuration.TopicPrefix}/{MTConnectMqttDocumentServer.SampleTopic}/{device.Uuid}";
                    if (formatResult.Content != null && formatResult.Content.Position > 0) formatResult.Content.Seek(0, SeekOrigin.Begin);

                    var messageBuilder = new MqttApplicationMessageBuilder();
                    //messageBuilder.WithRetainFlag(true);
                    messageBuilder.WithTopic(topic);
                    messageBuilder.WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce);
                    messageBuilder.WithPayload(formatResult.Content);
                    var message = messageBuilder.Build();

                    try
                    {
                        var publishResult = await _mqttClient.PublishAsync(message);
                        if (publishResult.IsSuccess)
                        {
                            Log(MTConnectLogLevel.Debug, $"Sample : Published to Topic ({topic})");
                        }
                        else
                        {
                            Log(MTConnectLogLevel.Warning, $"Sample : Error Publishing to Topic ({topic}) : {publishResult.ReasonCode} : {publishResult.ReasonString}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Log(MTConnectLogLevel.Warning, $"Sample : Error Publishing to Topic ({topic}) : {ex.Message}");
                    }
                }
            }
        }

        private async void AssetReceived(IDevice device, IAssetsResponseDocument responseDocument)
        {
            // async void handler: route any unhandled fault to the log
            // instead of the synchronization context.
            await AsyncVoidGuard.Run(
                async () => await AssetReceivedCore(device, responseDocument),
                ex => Log(MTConnectLogLevel.Warning, $"AssetReceived handler error : {ex.Message}"));
        }

        private async Task AssetReceivedCore(IDevice device, IAssetsResponseDocument responseDocument)
        {
            if (_mqttClient != null && _mqttClient.IsConnected && responseDocument != null && !responseDocument.Assets.IsNullOrEmpty())
            {
                var x = new List<KeyValuePair<string, string>>();
                x.Add(new KeyValuePair<string, string>("indentOutput", _configuration.IndentOutput.ToString()));

                foreach (var asset in responseDocument.Assets)
                {
                    var formatResult = EntityFormatter.Format(_configuration.DocumentFormat, asset, x);
                    if (formatResult.Success)
                    {
                        var topic = $"{_configuration.TopicPrefix}/{MTConnectMqttDocumentServer.AssetTopic}/{device.Uuid}/{asset.AssetId}";
                        if (formatResult.Content != null && formatResult.Content.Position > 0) formatResult.Content.Seek(0, SeekOrigin.Begin);

                        var messageBuilder = new MqttApplicationMessageBuilder();
                        messageBuilder.WithRetainFlag(true);
                        messageBuilder.WithTopic(topic);
                        messageBuilder.WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce);
                        messageBuilder.WithPayload(formatResult.Content);
                        var message = messageBuilder.Build();

                        try
                        {
                            var publishResult = await _mqttClient.PublishAsync(message);
                            if (publishResult.IsSuccess)
                            {
                                Log(MTConnectLogLevel.Debug, $"Asset : Published to Topic ({topic})");
                            }
                            else
                            {
                                Log(MTConnectLogLevel.Warning, $"Asset : Error Publishing to Topic ({topic}) : {publishResult.ReasonCode} : {publishResult.ReasonString}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Log(MTConnectLogLevel.Warning, $"Asset : Error Publishing to Topic ({topic}) : {ex.Message}");
                        }
                    }
                }
            }
        }


        private async Task PublishDevices()
        {
            if (_entityServer != null)
            {
                var devices = Agent.GetDevices();
                if (!devices.IsNullOrEmpty())
                {
                    foreach (var device in devices)
                    {
                        await _entityServer.PublishDevice(_mqttClient, device);
                    }
                }
            }
        }

        private async Task PublishCurrentObservations()
        {
            if (_entityServer != null)
            {
                var observations = Agent.GetCurrentObservations();
                await PublishObservations(observations);
            }
        }

        private async Task PublishObservations(IEnumerable<IObservationOutput> observations)
        {
            if (observations.IsNullOrEmpty()) return;

            // Single-pass grouping keeps the publish hot path O(n) in
            // the observation count; on large agents the catch-up after
            // a reconnect therefore stays bounded.
            foreach (var group in ObservationGrouper.GroupByDataItem(observations))
            {
                // Materialise each group's observations once: the
                // condition path needs to enumerate twice (test the
                // first item's category and rebuild every observation)
                // and re-iterating the IGrouping would re-iterate the
                // upstream source.
                var groupObservations = group.ToList();
                if (groupObservations.Count == 0) continue;

                var first = groupObservations[0];

                if (first.Category == DataItemCategory.CONDITION)
                {
                    // Conditions have multiple observations
                    var multipleObservations = new List<IObservation>(groupObservations.Count);
                    foreach (var observation in groupObservations)
                    {
                        multipleObservations.Add(CloneAsObservation(observation));
                    }

                    await _entityServer.PublishObservations(_mqttClient, multipleObservations);
                }
                else
                {
                    await _entityServer.PublishObservation(_mqttClient, CloneAsObservation(first));
                }
            }
        }

        private static Observation CloneAsObservation(IObservationOutput source)
        {
            var x = new Observation();
            x.DeviceUuid = source.DeviceUuid;
            x.DataItemId = source.DataItemId;
            x.DataItem = source.DataItem;
            x.Name = source.Name;
            x.Category = source.Category;
            x.Type = source.Type;
            x.SubType = source.SubType;
            x.Representation = source.Representation;
            x.CompositionId = source.CompositionId;
            x.InstanceId = source.InstanceId;
            x.Sequence = source.Sequence;
            x.Timestamp = source.Timestamp;
            x.AddValues(source.Values);
            return x;
        }

        private async Task PublishAssets()
        {
            if (_entityServer != null)
            {
                var assets = Agent.GetAssets();
                if (!assets.IsNullOrEmpty())
                {
                    foreach (var asset in assets)
                    {
                        await _entityServer.PublishAsset(_mqttClient, asset);
                    }
                }
            }
        }


        private async void AgentDeviceAdded(object sender, IDevice device)
        {
            // async void handlers must not throw; route any fault to
            // the log instead of the synchronization context.
            await AsyncVoidGuard.Run(
                async () =>
                {
                    if (_entityServer != null) await _entityServer.PublishDevice(_mqttClient, device);
                },
                ex => Log(MTConnectLogLevel.Warning, $"AgentDeviceAdded handler error : {ex.Message}"));
        }

        private async void AgentObservationAdded(object sender, IObservation observation)
        {
            // Increment the in-flight handler counter so OnStop's drain
            // window observes this body before the final
            // last-sent-sequence flush runs; the decrement at the end of
            // the body is unconditional (try / finally) so a throw still
            // releases the count.
            Interlocked.Increment(ref _handlerActiveCount);
            try
            {
                // async void handlers must not throw; route any fault to
                // the log instead of the synchronization context.
                await AsyncVoidGuard.Run(
                    async () =>
                    {
                        if (_configuration.DurableRelay)
                        {
                            Interlocked.Increment(ref _totalIncomingObservations);

                            // No disk read on the hot path: the persister
                            // is seeded from disk at OnStartAfterLoad and
                            // updated in-memory by RecordLastSentSequence
                            // below, so observation events do not stall
                            // on File.ReadAllText.

                            if (_entityServer != null)
                            {
                                if (observation.Category == DataItemCategory.CONDITION)
                                {
                                    var conditionObservations = Agent.GetCurrentObservations(observation.DeviceUuid, observation.DataItemId);
                                    var result = await _entityServer.PublishObservations(_mqttClient, conditionObservations.OfType<IObservation>());
                                    if (result != null && result.IsSuccess && conditionObservations != null && conditionObservations.Any())
                                    {
                                        RecordLastSentSequence(conditionObservations.Max(o => o.Sequence));
                                    }
                                }
                                else
                                {
                                    var result = await _entityServer.PublishObservation(_mqttClient, observation);
                                    if (result != null && result.IsSuccess)
                                    {
                                        RecordLastSentSequence(observation.Sequence);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (_entityServer != null)
                            {
                                if (observation.Category == DataItemCategory.CONDITION)
                                {
                                    var conditionObservations = Agent.GetCurrentObservations(observation.DeviceUuid, observation.DataItemId);
                                    await PublishObservations(conditionObservations);
                                }
                                else
                                {
                                    await _entityServer.PublishObservation(_mqttClient, observation);
                                }
                            }
                        }
                    },
                    ex => Log(MTConnectLogLevel.Warning, $"AgentObservationAdded handler error : {ex.Message}"));
            }
            finally
            {
                Interlocked.Decrement(ref _handlerActiveCount);
            }
        }

        private async void AgentAssetAdded(object sender, IAsset asset)
        {
            // async void handlers must not throw; route any fault to
            // the log instead of the synchronization context.
            await AsyncVoidGuard.Run(
                async () =>
                {
                    if (_entityServer != null) await _entityServer.PublishAsset(_mqttClient, asset);
                },
                ex => Log(MTConnectLogLevel.Warning, $"AgentAssetAdded handler error : {ex.Message}"));
        }


        private string GetAgentAvailableTopic()
        {
            if (Agent == null || _configuration == null) return null;

            return AvailabilityTopic.Build(_configuration.TopicPrefix, Agent.Uuid);
        }
    }
}
