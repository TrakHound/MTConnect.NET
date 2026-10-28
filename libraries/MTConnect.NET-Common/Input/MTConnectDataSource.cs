// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Logging;
using MTConnect.Observations;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Input
{
    /// <summary>
    /// Engine used to handle a worker thread running at an Interval used to organize reading from a Data Source (ex. PLC) and writing to an Adapter
    /// </summary>
    public abstract class MTConnectDataSource : IMTConnectDataSource
    {
        private CancellationTokenSource _stop;
        private bool _isStarted;

        // Last values sent per DataItem key (used when SuppressUnchangedObservations is enabled)
        private readonly System.Collections.Concurrent.ConcurrentDictionary<string, ObservationValue[]> _lastValues = new System.Collections.Concurrent.ConcurrentDictionary<string, ObservationValue[]>();

        /// <summary>
        /// When true, an Observation whose values equal the last values sent for the same DataItem key is not raised
        /// again (the agent would discard it anyway after processing and validating it). Leave false for data sources
        /// with Discrete DataItems, where a repeated value is a new event.
        /// </summary>
        protected bool SuppressUnchangedObservations { get; set; }

        /// <summary>
        /// Optional check, set by the host, that reports whether a log level is enabled so per-read messages that would
        /// be discarded are not built. When not set, every level is treated as enabled.
        /// </summary>
        public Func<MTConnectLogLevel, bool> LogLevelEnabled { get; set; }

        /// <summary>
        /// Returns true when a log entry at <paramref name="logLevel"/> would be recorded.
        /// </summary>
        /// <param name="logLevel">The severity of the log entry.</param>
        protected bool IsLogEnabled(MTConnectLogLevel logLevel)
        {
            if (LogReceived == null) return false;

            var check = LogLevelEnabled;
            return check == null || check(logLevel);
        }


        /// <summary>
        /// The configuration that controls the polling interval and identifies the data source.
        /// </summary>
        public IDataSourceConfiguration Configuration { get; set; }


        /// <summary>
        /// Raised when the data source produces a new Observation to be written to the Adapter.
        /// </summary>
        public event EventHandler<IObservationInput> ObservationAdded;

        /// <summary>
        /// Raised when the data source produces a new Asset to be written to the Adapter.
        /// </summary>
        public event EventHandler<IAssetInput> AssetAdded;

        /// <summary>
        /// Raised when the data source produces a new Device to be written to the Adapter.
        /// </summary>
        public event EventHandler<IDeviceInput> DeviceAdded;

        /// <summary>
        /// Raised when the data source emits a log message.
        /// </summary>
        public event MTConnectLogEventHandler LogReceived;


        /// <summary>
        /// Initializes a new data source with a default <see cref="DataSourceConfiguration"/>.
        /// </summary>
        public MTConnectDataSource()
        {
            Configuration = new DataSourceConfiguration();
        }


        /// <summary>
        /// Called when the data source is started. Override to perform connection or initialization work.
        /// </summary>
        protected virtual void OnStart() { }

        /// <summary>
        /// Called when the data source is stopped. Override to release resources or close connections.
        /// </summary>
        protected virtual void OnStop() { }

        /// <summary>
        /// Called on each polling cycle. Override to perform synchronous reads from the data source.
        /// </summary>
        protected virtual void OnRead() { }

        /// <summary>
        /// Called on each polling cycle after <see cref="OnRead"/>. Override to perform asynchronous reads from the data source.
        /// </summary>
        /// <returns>A task that completes when the read has finished.</returns>
        protected virtual Task OnReadAsync() { return Task.CompletedTask; }


        /// <summary>
        /// Starts the worker thread that polls the data source at the configured interval. No effect if already started.
        /// </summary>
        public void Start()
        {
            if (!_isStarted)
            {
                _isStarted = true;

                _stop = new CancellationTokenSource();

                _lastValues.Clear();

                OnStart();

                // Polling runs on its own thread: OnRead implementations make blocking (socket / native) calls,
                // which would otherwise hold thread-pool threads for the whole read
                var stopToken = _stop.Token;
                var thread = new Thread(() => Worker(stopToken)) { IsBackground = true, Name = $"MTConnectDataSource:{GetType().Name}" };
                thread.Start();
            }
        }

        /// <summary>
        /// Stops the worker thread and ends polling of the data source. No effect if not started.
        /// </summary>
        public void Stop()
        {
            if (_isStarted)
            {
                if (_stop != null) _stop.Cancel();
                _isStarted = false;

                OnStop();
            }
        }

        private void Worker(CancellationToken stopToken)
        {
            if (Configuration != null)
            {
                try
                {
                    while (!stopToken.IsCancellationRequested)
                    {
                        try
                        {
                            OnRead();
                            OnReadAsync().GetAwaiter().GetResult();
                        }
                        catch (OperationCanceledException) { }
                        catch { }

                        // Wait for the ReadInterval (returns early when stopped)
                        stopToken.WaitHandle.WaitOne(Math.Max(0, Configuration.ReadInterval));
                    }
                }
                catch { }
            }
        }


        /// <summary>
        /// Queues an Observation for the specified DataItem using the current time as the timestamp.
        /// </summary>
        /// <param name="dataItem">The DataItem the Observation applies to. Ignored when null or without an Id.</param>
        /// <param name="resultValue">The Result value to report.</param>
        public void AddObservation(IDataItem dataItem, object resultValue)
        {
            if (dataItem != null && !string.IsNullOrEmpty(dataItem.Id))
            {
                var observation = new ObservationInput(dataItem.Id, resultValue);
                AddObservation(observation);
            }
        }

        /// <summary>
        /// Queues an Observation for the specified DataItem at the given timestamp.
        /// </summary>
        /// <param name="dataItem">The DataItem the Observation applies to. Ignored when null or without an Id.</param>
        /// <param name="resultValue">The Result value to report.</param>
        /// <param name="timestamp">The observation timestamp as UnixTime in milliseconds.</param>
        public void AddObservation(IDataItem dataItem, object resultValue, long? timestamp = null)
        {
            if (dataItem != null && !string.IsNullOrEmpty(dataItem.Id))
            {
                var observation = new ObservationInput(dataItem.Id, resultValue, timestamp.Value);
                AddObservation(observation);
            }
        }

        /// <summary>
        /// Queues an Observation for the specified DataItem at the given timestamp.
        /// </summary>
        /// <param name="dataItem">The DataItem the Observation applies to. Ignored when null or without an Id.</param>
        /// <param name="resultValue">The Result value to report.</param>
        /// <param name="timestamp">The observation timestamp.</param>
        public void AddObservation(IDataItem dataItem, object resultValue, DateTime? timestamp = null)
        {
            if (dataItem != null && !string.IsNullOrEmpty(dataItem.Id))
            {
                var observation = new ObservationInput(dataItem.Id, resultValue, timestamp.Value);
                AddObservation(observation);
            }
        }

        /// <summary>
        /// Queues an Observation for the specified DataItem key using the current time as the timestamp.
        /// </summary>
        /// <param name="dataItemKey">The (ID, Name, or Source) of the DataItem the Observation applies to.</param>
        /// <param name="resultValue">The Result value to report.</param>
        public void AddObservation(string dataItemKey, object resultValue)
        {
            var observation = new ObservationInput(dataItemKey, resultValue);
            AddObservation(observation);
        }

        /// <summary>
        /// Queues an Observation for the specified DataItem key at the given timestamp.
        /// </summary>
        /// <param name="dataItemKey">The (ID, Name, or Source) of the DataItem the Observation applies to.</param>
        /// <param name="resultValue">The Result value to report.</param>
        /// <param name="timestamp">The observation timestamp as UnixTime in milliseconds.</param>
        public void AddObservation(string dataItemKey, object resultValue, long? timestamp = null)
        {
            var observation = new ObservationInput(dataItemKey, resultValue, timestamp.Value);
            AddObservation(observation);
        }

        /// <summary>
        /// Queues an Observation for the specified DataItem key at the given timestamp.
        /// </summary>
        /// <param name="dataItemKey">The (ID, Name, or Source) of the DataItem the Observation applies to.</param>
        /// <param name="resultValue">The Result value to report.</param>
        /// <param name="timestamp">The observation timestamp.</param>
        public void AddObservation(string dataItemKey, object resultValue, DateTime? timestamp = null)
        {
            var observation = new ObservationInput(dataItemKey, resultValue, timestamp.Value);
            AddObservation(observation);
        }

        /// <summary>
        /// Queues a fully constructed Observation, raising <see cref="ObservationAdded"/>.
        /// </summary>
        /// <param name="observation">The Observation to report.</param>
        public void AddObservation(IObservationInput observation)
        {
            if (observation == null) return;

            if (SuppressUnchangedObservations && observation.DataItemKey != null)
            {
                var values = observation.Values != null ? System.Linq.Enumerable.ToArray(observation.Values) : new ObservationValue[0];
                var key = observation.DeviceKey != null ? observation.DeviceKey + ":" + observation.DataItemKey : observation.DataItemKey;

                if (_lastValues.TryGetValue(key, out var lastValues) && ValuesEqual(lastValues, values)) return;
                _lastValues[key] = values;
            }

            if (ObservationAdded != null) ObservationAdded.Invoke(this, observation);
        }

        private static bool ValuesEqual(ObservationValue[] a, ObservationValue[] b)
        {
            if (a.Length != b.Length) return false;
            for (var i = 0; i < a.Length; i++)
            {
                if (a[i].Key != b[i].Key || a[i].Value != b[i].Value) return false;
            }
            return true;
        }


        /// <summary>
        /// Queues an Asset for reporting to the Adapter.
        /// </summary>
        /// <param name="asset">The Asset to report. Ignored when null.</param>
        public void AddAsset(IAsset asset)
        {
            if (asset != null)
            {
                AddAsset(new AssetInput(asset));
            }
        }

        /// <summary>
        /// Queues a fully constructed Asset Input, raising <see cref="AssetAdded"/>.
        /// </summary>
        /// <param name="asset">The Asset Input to report.</param>
        public void AddAsset(IAssetInput asset)
        {
            if (AssetAdded != null) AssetAdded.Invoke(this, asset);
        }


        /// <summary>
        /// Queues a Device for reporting to the Adapter.
        /// </summary>
        /// <param name="device">The Device to report. Ignored when null.</param>
        public void AddDevice(IDevice device)
        {
            if (device != null)
            {
                AddDevice(new DeviceInput(device));
            }
        }

        /// <summary>
        /// Queues a fully constructed Device Input, raising <see cref="DeviceAdded"/>.
        /// </summary>
        /// <param name="device">The Device Input to report.</param>
        public void AddDevice(IDeviceInput device)
        {
            if (DeviceAdded != null) DeviceAdded.Invoke(this, device);
        }


        /// <summary>
        /// Emits a log message to subscribers of <see cref="LogReceived"/>.
        /// </summary>
        /// <param name="logLevel">The severity of the log message.</param>
        /// <param name="message">The log message text.</param>
        protected void Log(MTConnectLogLevel logLevel, string message)
        {
            if (LogReceived != null) LogReceived.Invoke(this, logLevel, message);
        }
    }
}
