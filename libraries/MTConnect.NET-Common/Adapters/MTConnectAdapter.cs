// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using MTConnect.Devices;
using MTConnect.Input;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Adapters
{
    /// <summary>
    /// An Adapter for communicating with an MTConnect Agent.
    /// Uses a queue to collect changes to Observations and sends the most recent changes at the specified interval.
    /// </summary>
    public class MTConnectAdapter : IMTConnectAdapter
    {
        private readonly object _lock = new object();
        private readonly int? _interval;
        private readonly bool _bufferEnabled;

        private readonly Dictionary<string, IDevice> _devices = new Dictionary<string, IDevice>();

        private readonly Dictionary<string, IAssetInput> _currentAssets = new Dictionary<string, IAssetInput>();
        private readonly Dictionary<string, IAssetInput> _lastAssets = new Dictionary<string, IAssetInput>();
        private readonly Dictionary<byte[], bool> _sentAssets = new Dictionary<byte[], bool>();

        private readonly Dictionary<string, IObservationInput> _currentObservations = new Dictionary<string, IObservationInput>();
        private readonly Dictionary<string, IObservationInput> _lastObservations = new Dictionary<string, IObservationInput>();
        private readonly ItemQueue<IObservationInput> _observationsBuffer = new ItemQueue<IObservationInput>();
        private readonly Dictionary<byte[], bool> _sentObservations = new Dictionary<byte[], bool>();

        //private readonly Dictionary<string, IObservationInput> _currentAssets = new Dictionary<string, IObservationInput>();
        //private readonly Dictionary<string, IObservationInput> _lastAssets = new Dictionary<string, IObservationInput>();
        //private readonly ItemQueue<IObservationInput> _observationsBuffer = new ItemQueue<IObservationInput>();
        //private readonly Dictionary<byte[], bool> _sentObservations = new Dictionary<byte[], bool>();


        //private readonly Dictionary<string, IConditionObservationInput> _currentConditionObservations = new Dictionary<string, IConditionObservationInput>();
        //private readonly Dictionary<string, IConditionObservationInput> _lastConditionObservations = new Dictionary<string, IConditionObservationInput>();
        //private readonly ItemQueue<IConditionObservationInput> _conditionObservationsBuffer = new ItemQueue<IConditionObservationInput>();
        //private readonly Dictionary<byte[], bool> _sentConditionObservations = new Dictionary<byte[], bool>();


        private CancellationTokenSource _stop;
        protected CancellationTokenSource StopToken => _stop;


        /// <summary>
        /// Get a unique identifier for the Adapter
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// The Name or UUID of the Device to create a connection for
        /// </summary>
        public string DeviceKey { get; }


        ///// <summary>
        ///// The TCP Port used for communication
        ///// </summary>
        //public int Port { get; }

        ///// <summary>
        ///// The heartbeat used to maintain a connection between the Adapter and the Agent
        ///// </summary>
        //public int Heartbeat { get; }

        ///// <summary>
        ///// The amount of time (in milliseconds) to allow for a connection attempt to the Agent
        ///// </summary>
        //public int Timeout { get; set; }

        ///// <summary>
        ///// Use multiline Assets
        ///// </summary>
        //public bool MultilineAssets { get; set; }

        ///// <summary>
        ///// Use multiline Devices
        ///// </summary>
        //public bool MultilineDevices { get; set; }

        /// <summary>
        /// Determines whether to filter out duplicate data
        /// </summary>
        public bool FilterDuplicates { get; set; }

        /// <summary>
        /// Determines whether to output Timestamps for each SHDR line
        /// </summary>
        public bool OutputTimestamps { get; set; }

        public Func<IEnumerable<IObservationInput>, bool> WriteObservationsFunction { get; set; }

        public Func<IEnumerable<IAssetInput>, bool> WriteAssetsFunction { get; set; }

        public Func<IEnumerable<IDevice>, bool> WriteDevicesFunction { get; set; }


        //public Func<IEnumerable<IConditionObservationInput>, bool> WriteConditionObservationsFunction { get; set; }


        ///// <summary>
        ///// Raised when a new Agent connection is established. Includes the AgentClient ID as an argument.
        ///// </summary>
        //public event EventHandler<string> AgentConnected;

        ///// <summary>
        ///// Raised when an existing Agent connection is disconnected. Includes the AgentClient ID as an argument.
        ///// </summary>
        //public event EventHandler<string> AgentDisconnected;

        ///// <summary>
        ///// Raised when an error occurs during an existing Agent connection. Includes the AgentClient ID as an argument.
        ///// </summary>
        //public event EventHandler<string> AgentConnectionError;


        ///// <summary>
        ///// Raised when a Ping request message is received from an Agent. Includes the AgentClient ID as an argument.
        ///// </summary>
        //public event EventHandler<string> PingReceived;

        ///// <summary>
        ///// Raised when a Pong response message is sent to an Agent. Includes the AgentClient ID as an argument.
        ///// </summary>
        //public event EventHandler<string> PongSent;

        ///// <summary>
        ///// Raised when a new line is sent to the Agent. Includes the AgentClient ID and the Line sent as an argument.
        ///// </summary>
        //public event EventHandler<AdapterEventArgs> LineSent;

        /// <summary>
        /// Raised when new data is sent to the Agent. Includes the AgentClient ID and the Line sent as an argument.
        /// </summary>
        public event EventHandler<AdapterEventArgs> DataSent;

        /// <summary>
        /// Raised when an error occurs when sending a new line to the Agent. Includes the AgentClient ID and the Error message as an argument.
        /// </summary>
        public event EventHandler<AdapterEventArgs> SendError;


        public MTConnectAdapter(int? interval = 0, bool bufferEnabled = false)
        {
            _interval = interval;
            _bufferEnabled = bufferEnabled;

            FilterDuplicates = true;
            OutputTimestamps = true;
        }


        /// <summary>
        /// Starts the Adapter to begins listening for Agent connections as well as starts the Queue for collecting and sending data to the Agent(s).
        /// </summary>
        public void Start()
        {
            _stop = new CancellationTokenSource();

            // Start Agent Connection Listener
            //_connectionListener.Start(_stop.Token);


            if (_interval != null && _interval >= 0)
            {
                // Start Write Queue
                _ = Task.Run(() => Worker(StopToken.Token));
            }

            // Call Overridable Method
            OnStart();
        }

        /// <summary>
        /// Stops the adapter which also stops listening for Agent connections, disconnects any existing Agent connections, and stops the Queue for sending data to the Agent(s).
        /// </summary>
        public void Stop()
        {
            if (_stop != null) _stop.Cancel();
            //_connectionListener.Stop();

            // Call Overridable Method
            OnStop();
        }


        protected virtual void OnStart() { }

        protected virtual void OnStop() { }


        private async Task Worker(CancellationToken cancellationToken)
        {
            try
            {
                do
                {
                    int interval = Math.Max(1, _interval.Value); // Set Minimum of 1ms to prevent high CPU usage

                    var stpw = System.Diagnostics.Stopwatch.StartNew();

                    // Call Overridable Method
                    OnIntervalElapsed();

                    stpw.Stop();

                    if (stpw.ElapsedMilliseconds < interval)
                    {
                        var waitInterval = interval - (int)stpw.ElapsedMilliseconds;

                        await Task.Delay(waitInterval, cancellationToken);
                    }

                } while (!cancellationToken.IsCancellationRequested);
            }
            catch (TaskCanceledException) { }
            catch (Exception) { }
        }

        protected virtual void OnIntervalElapsed()
        {
            if (_bufferEnabled)
            {
                SendBuffer();
            }
            else
            {
                SendChanged();
            }
        }


        /// <summary>
        /// Set all items to Unavailable
        /// </summary>
        public void SetUnavailable(long timestamp = 0)
        {
            SetObservationsUnavailable(timestamp);
            //SetConditionObservationsUnavailable(timestamp);
            //SetDataItemsUnavailable(timestamp);
            //SetMessagesUnavailable(timestamp);
            //SetConditionsUnavailable(timestamp);
            //SetTimeSeriesUnavailable(timestamp);
            //SetDataSetsUnavailable(timestamp);
            //SetTablesUnavailable(timestamp);
        }


        #region "Write"

        protected bool Write(IObservationInput observation)
        {
            if (observation != null)
            {
                return Write(new List<IObservationInput> { observation });
            }

            return false;
        }

        protected bool Write(IEnumerable<IObservationInput> observations)
        {
            if (WriteObservationsFunction != null)
            {
                return WriteObservationsFunction(observations);
            }

            return true;
        }


        //protected bool Write(IConditionObservationInput observation)
        //{
        //    if (observation != null)
        //    {
        //        return Write(new List<IConditionObservationInput> { observation });
        //    }

        //    return false;
        //}

        //protected bool Write(IEnumerable<IConditionObservationInput> observations)
        //{
        //    if (WriteObservationsFunction != null)
        //    {
        //        return WriteConditionObservationsFunction(observations);
        //    }

        //    return true;
        //}

        #endregion

        #region "Send"

        /// <summary>
        /// Sends all Items that have changed since last sent to the Agent
        /// </summary>
        public void SendChanged()
        {
            WriteChangedObservations();
            //WriteChangedConditionObservations();
            //WriteChangedDataItems();
            //WriteChangedMessages();
            //WriteChangedConditions();
            //WriteChangedTimeSeries();
            //WriteChangedDataSets();
            //WriteChangedTables();
            WriteChangedAssets();

            // Call Overridable Method
            OnChangedSent();
        }

        /// <summary>
        /// Sends all of the last sent Items, Assets, and Devices to the Agent. This can be used upon reconnection to the Agent
        /// </summary>
        public void SendLast(long timestamp = 0)
        {
            WriteLastObservations(timestamp);
            //WriteLastConditionObservations(timestamp);
            //WriteLastDataItems(timestamp);
            //WriteLastMessages(timestamp);
            //WriteLastConditions(timestamp);
            //WriteLastTimeSeries(timestamp);
            //WriteLastDataSets(timestamp);
            //WriteLastTables(timestamp);
            WriteAllAssets();
            //WriteAllDevices();

            // Call Overridable Method
            OnLastSent();
        }


        public void SendBuffer()
        {
            WriteBufferObservations();
            //WriteBufferConditionObservations();
            //WriteChangedDataItems();
            //WriteChangedMessages();
            //WriteChangedConditions();
            //WriteChangedTimeSeries();
            //WriteChangedDataSets();
            //WriteChangedTables();
            //WriteChangedAssets();

            WriteChangedAssets();

            //// Call Overridable Method
            //OnChangedSent();
        }


        protected virtual void OnChangedSent() { }

        protected virtual void OnLastSent() { }

        #endregion

        #region "Observations"

        protected virtual void OnObservationAdd(IObservationInput observation) { }


        public void AddObservation(string dataItemKey, object value)
        {
            AddObservation(dataItemKey, value, UnixDateTime.Now);
        }

        public void AddObservation(string dataItemKey, object value, DateTime timestamp)
        {
            AddObservation(dataItemKey, value, timestamp.ToUnixTime());
        }

        public void AddObservation(string dataItemKey, object value, long timestamp)
        {
            AddObservation(new ObservationInput(dataItemKey, value, timestamp));
        }

        public void AddObservation(IObservationInput observation)
        {
            if (observation != null)
            {
                var newObservation = new ObservationInput(observation);

                // Set the DeviceKey
                newObservation.DeviceKey = DeviceKey;

                // Set Timestamp (if not already set)
                if (newObservation.Timestamp <= 0) newObservation.Timestamp = UnixDateTime.Now;

                // Get the Current Observation (if exists)
                IObservationInput currentObservation;
                lock (_lock) _currentObservations.TryGetValue(newObservation.DataItemKey, out currentObservation);

                // Check to see if new Observation is the same as the Current
                var add = true;
                if (currentObservation != null && FilterDuplicates)
                {
                    add = !ObjectExtensions.ByteArraysEqual(newObservation.ChangeId, currentObservation.ChangeId);
                }

                if (add)
                {
                    // Add to Current
                    lock (_lock)
                    {
                        _currentObservations.Remove(newObservation.DataItemKey);
                        _currentObservations.Add(newObservation.DataItemKey, newObservation);
                    }

                    // If using Buffer, Add to Buffer
                    
                    
                    // -- Need to add Mode property?


                    _observationsBuffer.Add(CreateUniqueId(newObservation), newObservation);

                    // Call Overridable Method
                    OnObservationAdd(newObservation);
                }
            }
        }

        public void AddObservations(IEnumerable<IObservationInput> observations)
        {
            if (!observations.IsNullOrEmpty())
            {
                foreach (var observation in observations)
                {
                    AddObservation(observation);
                }
            }
        }


        public bool SendObservation(string dataItemId, object value)
        {
            return SendObservation(dataItemId, value, UnixDateTime.Now);
        }

        public bool SendObservation(string dataItemId, object value, DateTime timestamp)
        {
            return SendObservation(dataItemId, value, timestamp.ToUnixTime());
        }

        public bool SendObservation(string dataItemId, object value, long timestamp)
        {
            return SendObservation(new ObservationInput(dataItemId, value, timestamp));
        }

        public bool SendObservation(IObservationInput observation)
        {
            if (observation != null)
            {
                var newObservation = new ObservationInput(observation);

                // Set the DeviceKey
                newObservation.DeviceKey = DeviceKey;

                // Set Timestamp (if not already set)
                if (newObservation.Timestamp <= 0) newObservation.Timestamp = UnixDateTime.Now;
                //if (!OutputTimestamps) newDataItem.Timestamp = 0;
                //else /*if (newDataItem.Timestamp <= 0) newDataItem.Timestamp = UnixDateTime.Now;*/

                // Remove from Current
                lock (_lock) _currentObservations.Remove(newObservation.DataItemKey);

                // Call Overridable Method
                OnObservationAdd(newObservation);

                //// Create SHDR string to send
                //var sendItem = new ShdrDataItem(newObservation);
                //if (!OutputTimestamps) sendItem.Timestamp = 0;
                //var shdrLine = sendItem.ToString();

                var success = Write(newObservation);
                if (success)
                {
                   // Update Last Sent DataItems
                    UpdateLastObservations(new List<IObservationInput> { newObservation });
                }

                return success;
            }

            return false;
        }

        public bool SendObservations(IEnumerable<IObservationInput> dataItems)
        {
            var success = true;

            if (!dataItems.IsNullOrEmpty())
            {
                foreach (var dataItem in dataItems)
                {
                    if (!SendObservation(dataItem)) success = false;
                }

                return success;
            }

            return false;
        }


        protected void UpdateLastObservations(IEnumerable<IObservationInput> observations)
        {
            if (!observations.IsNullOrEmpty())
            {
                // Find the most recent Observation for each DataItemKey
                var dataItemKeys = observations.Select(o => o.DataItemKey).Distinct();
                foreach (var dataItemKey in dataItemKeys)
                {
                    var keyObservations = observations.Where(o => o.DataItemKey == dataItemKey);
                    var mostRecent = keyObservations.OrderByDescending(o => o.Timestamp).FirstOrDefault();
                    if (mostRecent != null)
                    {
                        var lastObservation = new ObservationInput(mostRecent);

                        lock (_lock)
                        {
                            _lastObservations.Remove(lastObservation.DataItemKey);
                            _lastObservations.Add(lastObservation.DataItemKey, lastObservation);
                        }
                    }
                }
            }
        }


        protected bool WriteChangedObservations()
        {
            // Get a list of all Current Observations
            List<IObservationInput> dataItems;
            lock (_lock)
            {
                // Get List of DataItems that need to be Updated
                dataItems = new List<IObservationInput>();
                var items = _currentObservations.Values;
                foreach (var item in items)
                {
                    var isSent = false;
                    if (_sentObservations.ContainsKey(item.ChangeId)) isSent = _sentObservations[item.ChangeId];

                    if (!isSent)
                    {
                        _sentObservations.Remove(item.ChangeId);
                        _sentObservations.Add(item.ChangeId, true);

                        var sendItem = new ObservationInput(item);
                        if (!OutputTimestamps) sendItem.Timestamp = 0;
                        dataItems.Add(sendItem);
                    }
                }
            }

            if (!dataItems.IsNullOrEmpty())
            {
                var success = Write(dataItems);
                //if (success)
                //{
                // Update Last Sent DataItems
                UpdateLastObservations(dataItems);
                //}

                return success;
            }

            return false;
        }

        protected bool WriteLastObservations(long timestamp = 0)
        {
            // Get a list of all Last Obserations
            IEnumerable<IObservationInput> dataItems;
            lock (_lock) dataItems = _lastObservations.Values.ToList();

            if (!dataItems.IsNullOrEmpty())
            {
                var sendItems = new List<IObservationInput>();
                foreach (var dataItem in dataItems)
                {
                    var sendItem = new ObservationInput(dataItem);
                    if (!OutputTimestamps) sendItem.Timestamp = 0;
                    sendItems.Add(sendItem);
                }

                // Create SHDR string to send
                //var shdrLine = ShdrDataItem.ToString(sendItems);
                var success = Write(sendItems);
                //if (success)
                //{
                // Update Last Sent DataItems
                UpdateLastObservations(dataItems);
                //}

                return success;
            }

            return false;
        }

        public bool WriteBufferObservations(int count = 1000)
        {
            var observations = _observationsBuffer.Take(count);
            if (!observations.IsNullOrEmpty())
            {
                var sendObservations = new List<IObservationInput>();
                foreach (var observation in observations)
                {
                    var sendObservation = new ObservationInput(observation);
                    if (!OutputTimestamps) sendObservation.Timestamp = 0;
                    sendObservations.Add(sendObservation);
                }

                var success = Write(sendObservations);
                if (success)
                {
                    // Update Last Sent DataItems
                    UpdateLastObservations(sendObservations);
                }
            }

            return false;
        }


        private void SetObservationsUnavailable(long timestamp = 0)
        {
            // Get a list of all Current Observations
            IEnumerable<IObservationInput> dataItems;
            lock (_lock) dataItems = _currentObservations.Values.ToList();

            if (!dataItems.IsNullOrEmpty())
            {
                var unavailableObservations = new List<IObservationInput>();
                var ts = timestamp > 0 ? timestamp : UnixDateTime.Now;

                // Set each Observation to Unavailable
                foreach (var item in dataItems)
                {
                    // Create new Unavailable Observation
                    var unavailableObservation = new ObservationInput();
                    unavailableObservation.DeviceKey = item.DeviceKey;
                    unavailableObservation.DataItemKey = item.DataItemKey;
                    unavailableObservation.Timestamp = ts;
                    unavailableObservation.Unavailable();
                    unavailableObservations.Add(unavailableObservation);
                }

                // Add Observations (only will add those that are changed)
                AddObservations(unavailableObservations);
            }
        }

        private static ulong CreateUniqueId(IObservationInput observationInput)
        {
            if (observationInput != null)
            {
                var hashBytes = observationInput.ChangeIdWithTimestamp;
                var hash = string.Concat(hashBytes.Select(b => b.ToString("x2")));
                return hash.GetUInt64Hash();
            }

            return 0;
        }

        #endregion

        #region "Assets"

        /// <summary>
        /// Add the specified MTConnect Asset and sends it to the Agent
        /// </summary>
        /// <param name="asset">The Asset to send</param>
        private void SendAsset(IAssetInput asset)
        {
            if (asset != null)
            {
                //// Set Timestamp (if not already set)
                //if (!OutputTimestamps) asset.Timestamp = 0;
                //else if (asset.Timestamp <= 0) asset.Timestamp = UnixDateTime.Now;

                lock (_lock)
                {
                    // Check to see if Asset already exists in list
                    var existing = _lastAssets.FirstOrDefault(o => o.Key == asset.AssetId).Value;
                    if (existing == null)
                    {
                        _lastAssets.Add(asset.AssetId, asset);
                    }
                    else
                    {
                        _lastAssets.Remove(asset.AssetId);
                        _lastAssets.Add(asset.AssetId, asset);
                    }
                }

                //var shdrLine = asset.ToString(MultilineAssets);
                //WriteLine(shdrLine);
            }
        }

        /// <summary>
        /// Add the specified MTConnect Assets and sends them to the Agent
        /// </summary>
        /// <param name="assets">The Assets to send</param>
        private void SendAssets(IEnumerable<IAssetInput> assets)
        {
            if (!assets.IsNullOrEmpty())
            {
                foreach (var item in assets)
                {
                    SendAsset(item);
                }
            }
        }


        /// <summary>
        /// Add the specified MTConnect Asset
        /// </summary>
        /// <param name="asset">The Asset to add</param>
        public void AddAsset(IAssetInput asset)
        {
            if (asset != null && asset.AssetId != null)
            {
                var newAsset = new AssetInput(asset);

                // Set Timestamp (if not already set)
                if (!OutputTimestamps) newAsset.Timestamp = 0;
                else if (newAsset.Timestamp <= 0) newAsset.Timestamp = UnixDateTime.Now;

                // Get the Current Asset (if exists)
                IAssetInput currentAsset;
                lock (_lock) _currentAssets.TryGetValue(newAsset.AssetId, out currentAsset);

                // Check to see if new Asset is the same as the Current
                var add = true;
                if (currentAsset != null && FilterDuplicates)
                {
                    add = !ObjectExtensions.ByteArraysEqual(newAsset.ChangeId, currentAsset.ChangeId);
                }

                if (add)
                {
                    // Add to Current
                    lock (_lock)
                    {
                        _currentAssets.Remove(newAsset.AssetId);
                        _currentAssets.Add(newAsset.AssetId, newAsset);
                    }

                    // Call Overridable Method
                    //OnAssetAdd(newAsset);
                }
            }
        }


        /// <summary>
        /// Add the specified MTConnect Assets
        /// </summary>
        /// <param name="assets">The Assets to send</param>
        private void AddAssets(IEnumerable<IAssetInput> assets)
        {
            if (!assets.IsNullOrEmpty())
            {
                foreach (var item in assets)
                {
                    AddAsset(item);
                }
            }
        }


        protected void UpdateLastAsset(IEnumerable<IAssetInput> assets)
        {
            if (!assets.IsNullOrEmpty())
            {
                // Find the most recent Asset for each AssetId
                var assetKeys = assets.Select(o => o.AssetId).Distinct();
                foreach (var assetKey in assetKeys)
                {
                    var keyAssets = assets.Where(o => o.AssetId == assetKey);
                    var mostRecent = keyAssets.OrderByDescending(o => o.Timestamp).FirstOrDefault();

                    lock (_lock)
                    {
                        _lastAssets.Remove(mostRecent.AssetId);
                        _lastAssets.Add(mostRecent.AssetId, mostRecent);
                    }
                }
            }
        }


        protected bool WriteChangedAssets()
        {
            // Get a list of all Current Assets
            List<IAssetInput> assets;
            lock (_lock)
            {
                // Get List of Table that need to be Updated
                assets = new List<IAssetInput>();
                var items = _currentAssets.Values;
                foreach (var item in items)
                {
                    var isSent = false;
                    if (_sentAssets.ContainsKey(item.ChangeId)) isSent = _sentAssets[item.ChangeId];

                    if (!isSent)
                    {
                        _sentAssets.Remove(item.ChangeId);
                        _sentAssets.Add(item.ChangeId, true);

                        assets.Add(item);
                    }
                }
            }

            if (!assets.IsNullOrEmpty())
            {
                var success = WriteAssetsFunction(assets);
                if (success)
                {
                    UpdateLastAsset(assets);
                }

                return success;
            }

            return false;
        }

        protected bool WriteAllAssets()
        {
            // Get a list of all Assets
            IEnumerable<IAssetInput> assets;
            lock (_lock) assets = _lastAssets.Values.ToList();

            if (!assets.IsNullOrEmpty())
            {
                var success = WriteAssetsFunction(assets);
                if (success)
                {
                    UpdateLastAsset(assets);
                }

                //bool success = false;

                //foreach (var item in assets)
                //{
                //    // Create SHDR string to send
                //    var shdrLine = item.ToString(MultilineAssets);
                //    success = WriteLine(shdrLine);
                //    if (!success) break;
                //}

                return success;
            }

            return false;
        }


        /// <summary>
        /// Remove the specified Asset using the SHDR command @REMOVE_ASSET@
        /// </summary>
        /// <param name="assetId">The AssetId of the Asset to remove</param>
        /// <param name="timestamp">The timestamp to send as part of the SHDR command</param>
        public void RemoveAsset(string assetId, long timestamp = 0)
        {
            //// Create SHDR string to send
            //var shdrLine = ShdrAsset.Remove(assetId, timestamp);

            //// Write line to stream
            //WriteLine(shdrLine);
        }

        /// <summary>
        /// Remove all Assets of the specified Type using the SHDR command @REMOVE_ALL_ASSETS@
        /// </summary>
        /// <param name="assetType">The Type of the Assets to remove</param>
        /// <param name="timestamp">The timestamp to send as part of the SHDR command</param>
        public void RemoveAllAssets(string assetType, long timestamp = 0)
        {
            //// Create SHDR string to send
            //var shdrLine = ShdrAsset.RemoveAll(assetType, timestamp);

            //// Write line to stream
            //WriteLine(shdrLine);
        }

        #endregion

        //#region "Devices"

        ///// <summary>
        ///// Add the specified MTConnect Device to the queue to be written to the adapter stream
        ///// </summary>
        ///// <param name="device">The Device to add</param>
        //public void AddDevice(Devices.IDevice device)
        //{
        //    AddDevice(new ShdrDevice(device));
        //}

        ///// <summary>
        ///// Add the specified MTConnect Device to the queue to be written to the adapter stream
        ///// </summary>
        ///// <param name="device">The Device to add</param>
        //private void AddDevice(ShdrDevice device)
        //{
        //    if (device != null)
        //    {
        //        lock (_lock)
        //        {
        //            // Check to see if Device already exists in list
        //            var existing = _devices.FirstOrDefault(o => o.Key == device.DeviceUuid).Value;
        //            if (existing == null)
        //            {
        //                _devices.Add(device.DeviceUuid, device);
        //            }
        //            else
        //            {
        //                if (existing.ChangeId != device.ChangeId)
        //                {
        //                    _devices.Remove(device.DeviceUuid);
        //                    _devices.Add(device.DeviceUuid, device);
        //                }
        //            }
        //        }

        //        var shdrLine = device.ToString();
        //        WriteLine(shdrLine);
        //    }
        //}

        ///// <summary>
        ///// Add the specified MTConnect Devices to the queue to be written to the adapter stream
        ///// </summary>
        ///// <param name="devices">The Devices to add</param>
        //public void AddDevices(IEnumerable<Devices.IDevice> devices)
        //{
        //    if (!devices.IsNullOrEmpty())
        //    {
        //        var items = new List<ShdrDevice>();
        //        foreach (var item in devices)
        //        {
        //            items.Add(new ShdrDevice(item));
        //        }

        //        AddDevices(items);
        //    }
        //}

        ///// <summary>
        ///// Add the specified MTConnect Devices to the queue to be written to the adapter stream
        ///// </summary>
        ///// <param name="devices">The Devices to add</param>
        //private void AddDevices(IEnumerable<ShdrDevice> devices)
        //{
        //    if (!devices.IsNullOrEmpty())
        //    {
        //        foreach (var item in devices)
        //        {
        //            AddDevice(item);
        //        }
        //    }
        //}

        //protected bool WriteAllDevices()
        //{
        //    // Get a list of all Devices
        //    IEnumerable<ShdrDevice> devices;
        //    lock (_lock) devices = _devices.Values.ToList();

        //    if (!devices.IsNullOrEmpty())
        //    {
        //        bool success = false;

        //        foreach (var item in devices)
        //        {
        //            // Create SHDR string to send
        //            var shdrLine = item.ToString();
        //            success = WriteLine(shdrLine);
        //            if (!success) break;
        //        }

        //        return success;
        //    }

        //    return false;
        //}


        ///// <summary>
        ///// Remove the specified Device using the SHDR command @REMOVE_ASSET@
        ///// </summary>
        ///// <param name="deviceId">The DeviceId of the Device to remove</param>
        ///// <param name="timestamp">The timestamp to send as part of the SHDR command</param>
        //public void RemoveDevice(string deviceId, long timestamp = 0)
        //{
        //    // Create SHDR string to send
        //    var shdrLine = ShdrDevice.Remove(deviceId, timestamp);

        //    // Write line to stream
        //    WriteLine(shdrLine);
        //}

        ///// <summary>
        ///// Remove all Devices of the specified Type using the SHDR command @REMOVE_ALL_ASSETS@
        ///// </summary>
        ///// <param name="deviceType">The Type of the Devices to remove</param>
        ///// <param name="timestamp">The timestamp to send as part of the SHDR command</param>
        //public void RemoveAllDevices(string deviceType, long timestamp = 0)
        //{
        //    // Create SHDR string to send
        //    var shdrLine = ShdrDevice.RemoveAll(deviceType, timestamp);

        //    // Write line to stream
        //    WriteLine(shdrLine);
        //}

        //#endregion




        //#region "Conditions"

        //protected virtual void OnConditionObservationAdd(IConditionObservationInput observation) { }


        //public void AddConditionObservation(IConditionObservationInput observation)
        //{
        //    if (observation != null)
        //    {
        //        var newObservation = new ConditionObservationInput(observation);

        //        // Set the DeviceKey
        //        newObservation.DeviceKey = DeviceKey;

        //        // Set Timestamp (if not already set)
        //        //if (newObservation.Timestamp <= 0) newObservation.Timestamp = UnixDateTime.Now;

        //        // Get the Current Condition Observation (if exists)
        //        IConditionObservationInput currentObservation;
        //        lock (_lock) _currentConditionObservations.TryGetValue(newObservation.DataItemKey, out currentObservation);

        //        // Check to see if new Observation is the same as the Current
        //        var add = true;
        //        if (currentObservation != null && FilterDuplicates)
        //        {
        //            add = !ObjectExtensions.ByteArraysEqual(newObservation.ChangeId, currentObservation.ChangeId);
        //        }

        //        if (add)
        //        {
        //            // Add to Current
        //            lock (_lock)
        //            {
        //                _currentConditionObservations.Remove(newObservation.DataItemKey);
        //                _currentConditionObservations.Add(newObservation.DataItemKey, newObservation);
        //            }

        //            // If using Buffer, Add to Buffer


        //            // -- Need to add Mode property?


        //            _conditionObservationsBuffer.Add(CreateUniqueId(newObservation), newObservation);

        //            // Call Overridable Method
        //            OnConditionObservationAdd(newObservation);
        //        }
        //    }
        //}

        //public void AddConditionObservations(IEnumerable<IConditionObservationInput> observations)
        //{
        //    if (!observations.IsNullOrEmpty())
        //    {
        //        foreach (var observation in observations)
        //        {
        //            AddConditionObservation(observation);
        //        }
        //    }
        //}


        //public bool SendConditionObservation(IConditionObservationInput observation)
        //{
        //    if (observation != null)
        //    {
        //        var newObservation = new ConditionObservationInput(observation);

        //        // Set the DeviceKey
        //        newObservation.DeviceKey = DeviceKey;

        //        // Set Timestamp (if not already set)
        //        //if (newObservation.Timestamp <= 0) newObservation.Timestamp = UnixDateTime.Now;
        //        //if (!OutputTimestamps) newDataItem.Timestamp = 0;
        //        //else /*if (newDataItem.Timestamp <= 0) newDataItem.Timestamp = UnixDateTime.Now;*/

        //        // Remove from Current
        //        lock (_lock) _currentConditionObservations.Remove(newObservation.DataItemKey);

        //        // Call Overridable Method
        //        OnConditionObservationAdd(newObservation);

        //        //// Create SHDR string to send
        //        //var sendItem = new ShdrDataItem(newObservation);
        //        //if (!OutputTimestamps) sendItem.Timestamp = 0;
        //        //var shdrLine = sendItem.ToString();

        //        var success = Write(newObservation);
        //        if (success)
        //        {
        //            // Update Last Sent DataItems
        //            UpdateLastConditionObservations(new List<IConditionObservationInput> { newObservation });
        //        }

        //        return success;
        //    }

        //    return false;
        //}

        //public bool SendConditionObservations(IEnumerable<IConditionObservationInput> dataItems)
        //{
        //    var success = true;

        //    if (!dataItems.IsNullOrEmpty())
        //    {
        //        foreach (var dataItem in dataItems)
        //        {
        //            if (!SendConditionObservation(dataItem)) success = false;
        //        }

        //        return success;
        //    }

        //    return false;
        //}


        //protected void UpdateLastConditionObservations(IEnumerable<IConditionObservationInput> observations)
        //{
        //    //if (!observations.IsNullOrEmpty())
        //    //{
        //    //    // Find the most recent Observation for each DataItemKey
        //    //    var dataItemKeys = observations.Select(o => o.DataItemKey).Distinct();
        //    //    foreach (var dataItemKey in dataItemKeys)
        //    //    {
        //    //        var keyObservations = observations.Where(o => o.DataItemKey == dataItemKey);
        //    //        var mostRecent = keyObservations.OrderByDescending(o => o.Timestamp).FirstOrDefault();
        //    //        if (mostRecent != null)
        //    //        {
        //    //            var lastObservation = new ConditionObservationInput(mostRecent);

        //    //            lock (_lock)
        //    //            {
        //    //                _lastConditionObservations.Remove(lastObservation.DataItemKey);
        //    //                _lastConditionObservations.Add(lastObservation.DataItemKey, lastObservation);
        //    //            }
        //    //        }
        //    //    }
        //    //}
        //}


        //protected bool WriteChangedConditionObservations()
        //{
        //    //// Get a list of all Current Condition Observations
        //    //List<IConditionObservationInput> dataItems;
        //    //lock (_lock)
        //    //{
        //    //    // Get List of DataItems that need to be Updated
        //    //    dataItems = new List<IConditionObservationInput>();
        //    //    var items = _currentConditionObservations.Values;
        //    //    foreach (var item in items)
        //    //    {
        //    //        var isSent = false;
        //    //        if (_sentConditionObservations.ContainsKey(item.ChangeId)) isSent = _sentConditionObservations[item.ChangeId];

        //    //        if (!isSent)
        //    //        {
        //    //            _sentConditionObservations.Remove(item.ChangeId);
        //    //            _sentConditionObservations.Add(item.ChangeId, true);

        //    //            var sendItem = new ConditionObservationInput(item);
        //    //            if (!OutputTimestamps) sendItem.Timestamp = 0;
        //    //            dataItems.Add(sendItem);
        //    //        }
        //    //    }
        //    //}

        //    //if (!dataItems.IsNullOrEmpty())
        //    //{
        //    //    var success = Write(dataItems);
        //    //    //if (success)
        //    //    //{
        //    //    // Update Last Sent DataItems
        //    //    UpdateLastConditionObservations(dataItems);
        //    //    //}

        //    //    return success;
        //    //}

        //    return false;
        //}

        //protected bool WriteLastConditionObservations(long timestamp = 0)
        //{
        //    // Get a list of all Last Condition Obserations
        //    IEnumerable<IConditionObservationInput> dataItems;
        //    lock (_lock) dataItems = _lastConditionObservations.Values.ToList();

        //    if (!dataItems.IsNullOrEmpty())
        //    {
        //        var sendItems = new List<IConditionObservationInput>();
        //        foreach (var dataItem in dataItems)
        //        {
        //            var sendItem = new ConditionObservationInput(dataItem);
        //            //if (!OutputTimestamps) sendItem.Timestamp = 0;
        //            sendItems.Add(sendItem);
        //        }

        //        // Create SHDR string to send
        //        //var shdrLine = ShdrDataItem.ToString(sendItems);
        //        var success = Write(sendItems);
        //        //if (success)
        //        //{
        //        // Update Last Sent DataItems
        //        UpdateLastConditionObservations(dataItems);
        //        //}

        //        return success;
        //    }

        //    return false;
        //}

        //public bool WriteBufferConditionObservations(int count = 1000)
        //{
        //    var observations = _conditionObservationsBuffer.Take(count);
        //    if (!observations.IsNullOrEmpty())
        //    {
        //        var sendObservations = new List<IConditionObservationInput>();
        //        foreach (var observation in observations)
        //        {
        //            var sendObservation = new ConditionObservationInput(observation);
        //            //if (!OutputTimestamps) sendObservation.Timestamp = 0;
        //            sendObservations.Add(sendObservation);
        //        }

        //        var success = Write(sendObservations);
        //        if (success)
        //        {
        //            // Update Last Sent DataItems
        //            UpdateLastConditionObservations(sendObservations);
        //        }
        //    }

        //    return false;
        //}


        //private void SetConditionObservationsUnavailable(long timestamp = 0)
        //{
        //    // Get a list of all Current Condition Observations
        //    IEnumerable<IConditionObservationInput> dataItems;
        //    lock (_lock) dataItems = _currentConditionObservations.Values.ToList();

        //    if (!dataItems.IsNullOrEmpty())
        //    {
        //        var unavailableObservations = new List<IConditionObservationInput>();
        //        var ts = timestamp > 0 ? timestamp : UnixDateTime.Now;

        //        // Set each Observation to Unavailable
        //        foreach (var item in dataItems)
        //        {
        //            // Create new Unavailable Observation
        //            var unavailableObservation = new ConditionObservationInput();
        //            unavailableObservation.DeviceKey = item.DeviceKey;
        //            unavailableObservation.DataItemKey = item.DataItemKey;
        //            unavailableObservation.Unavailable();
        //            unavailableObservations.Add(unavailableObservation);
        //        }

        //        // Add Observations (only will add those that are changed)
        //        AddConditionObservations(unavailableObservations);
        //    }
        //}

        //private static ulong CreateUniqueId(IConditionObservationInput observationInput)
        //{
        //    if (observationInput != null)
        //    {
        //        var hashBytes = observationInput.ChangeIdWithTimestamp;
        //        var hash = string.Concat(hashBytes.Select(b => b.ToString("x2")));
        //        return hash.GetUInt64Hash();
        //    }

        //    return 0;
        //}

        //#endregion

    }
}