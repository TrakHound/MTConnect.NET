// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Agents.Metrics;
using MTConnect.Assets;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Devices.DataItems;
using MTConnect.Observations;
using MTConnect.Observations.Input;
using MTConnect.Observations.Output;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MTConnect.Agents
{
    /// <summary>
    /// An Agent is the centerpiece of an MTConnect implementation. 
    /// It organizes and manages individual pieces of information published by one or more pieces of equipment.
    /// </summary>
    public class MTConnectAgent : IMTConnectAgent, IDisposable
    {
        private const int InformationUpdateInterval = 1000;

        private readonly IAgentConfiguration _configuration;
        private readonly MTConnectAgentInformation _information;

        protected readonly Dictionary<int, string> _deviceUuids = new Dictionary<int, string>();
        protected readonly Dictionary<int, string> _dataItemIds = new Dictionary<int, string>();
        protected int _lastDeviceIndex = 0;
        protected int _lastDataItemIndex = 0;

        protected readonly ConcurrentDictionary<string, string> _deviceKeys = new ConcurrentDictionary<string, string>(); // Resolves either the Device Name or UUID to the Device UUID
        protected readonly ConcurrentDictionary<string, IDevice> _devices = new ConcurrentDictionary<string, IDevice>(); // Resolves either the Device Name or UUID to the Device
        protected readonly ConcurrentDictionary<string, IEnumerable<IDataItem>> _deviceDataItems = new ConcurrentDictionary<string, IEnumerable<IDataItem>>();
        protected readonly ConcurrentDictionary<string, IEnumerable<string>> _deviceDataItemIds = new ConcurrentDictionary<string, IEnumerable<string>>();

        protected readonly ConcurrentDictionary<string, string> _dataItemKeys = new ConcurrentDictionary<string, string>(); // Caches DeviceUuid:DataItemKey to DeviceUuid:DataItemId
        protected readonly ConcurrentDictionary<string, IDataItem> _dataItems = new ConcurrentDictionary<string, IDataItem>(); // Key = DeviceUuid:DataItemId

        private readonly ConcurrentDictionary<string, IObservationInput> _currentObservations = new ConcurrentDictionary<string, IObservationInput>();
        private readonly ConcurrentDictionary<string, IEnumerable<IObservationInput>> _currentConditions = new ConcurrentDictionary<string, IEnumerable<IObservationInput>>();
        private readonly List<string> _assetIds = new List<string>();
        private MTConnectAgentMetrics _metrics;
        private readonly string _uuid;
        private readonly long _instanceId;
        private long _deviceModelChangeTime;
        private Version _version;
        private Version _mtconnectVersion;
        private Agent _agent;
        private string _sender;
        private System.Timers.Timer _informationUpdateTimer;
        private bool _updateInformation;


        #region "Properties"

        /// <summary>
        /// Gets a representation of the specific instance of the Agent.
        /// </summary>
        public long InstanceId => _instanceId;

        /// <summary>
        /// Gets the Configuration associated with the Agent
        /// </summary>
        public IAgentConfiguration Configuration => _configuration;

        /// <summary>
        /// Gets the Information associated with the Agent
        /// </summary>
        public MTConnectAgentInformation Information => _information;

        /// <summary>
        /// Gets the Metrics associated with the Agent
        /// </summary>
        public MTConnectAgentMetrics Metrics => _metrics;

        /// <summary>
        /// Gets or Sets the Agent Device that represents the MTConnect Agent
        /// </summary>
        public Agent Agent => _agent;

        /// <summary>
        /// Gets the UUID that uniquely identifies the Agent
        /// </summary>
        public string Uuid => _uuid;

        /// <summary>
        /// Gets the Agent Version
        /// </summary>
        public Version Version => _version;

        /// <summary>
        /// Gets the MTConnect Version that the Agent is using.
        /// </summary>
        public Version MTConnectVersion
        {
            get => _mtconnectVersion;
            set
            {
                var version = value;
                if (version == null) version = MTConnectVersions.Max;

                _mtconnectVersion = version;
                if (_agent != null) _agent.MTConnectVersion = _mtconnectVersion;
            }
        }

        /// <summary>
        /// Gets the Sender that is hosting the Agent
        /// </summary>
        public string Sender
        {
            get
            {
                if (string.IsNullOrEmpty(_sender)) _sender = System.Net.Dns.GetHostName();
                return _sender;
            }
        }

        /// <summary>
        /// A timestamp in 8601 format of the last update of the Device information for any device.
        /// </summary>
        public DateTime DeviceModelChangeTime => _deviceModelChangeTime.ToDateTime();

        public Func<ProcessObservation, IObservationInput> ProcessObservationFunction { get; set; }

        public Func<IAsset, IAsset> ProcessAssetFunction { get; set; }

        #endregion

        #region "Events"

        /// <summary>
        /// Raised when a new Device is added to the Agent
        /// </summary>
        public event EventHandler<IDevice> DeviceAdded;

        /// <summary>
        /// Raised when a new Observation is attempted to be added to the Agent
        /// </summary>
        public event EventHandler<IObservationInput> ObservationReceived;

        /// <summary>
        /// Raised when a new Observation is added to the Agent
        /// </summary>
        public event EventHandler<IObservation> ObservationAdded;

        /// <summary>
        /// Raised when a new Asset is attempted to be added to the Agent
        /// </summary>
        public event EventHandler<IAsset> AssetReceived;

        /// <summary>
        /// Raised when a new Asset is added to the Agent
        /// </summary>
        public event EventHandler<IAsset> AssetAdded;


        /// <summary>
        /// Raised when an Invalid Component is Added
        /// </summary>
        public event MTConnectComponentValidationHandler InvalidComponentAdded;

        /// <summary>
        /// Raised when an Invalid Composition is Added
        /// </summary>
        public event MTConnectCompositionValidationHandler InvalidCompositionAdded;

        /// <summary>
        /// Raised when an Invalid DataItem is Added
        /// </summary>
        public event MTConnectDataItemValidationHandler InvalidDataItemAdded;

        /// <summary>
        /// Raised when an Invalid Observation is Added
        /// </summary>
        public event MTConnectObservationValidationHandler InvalidObservationAdded;

        /// <summary>
        /// Raised when an Invalid Asset is Added
        /// </summary>
        public event MTConnectAssetValidationHandler InvalidAssetAdded;

        #endregion

        #region "Constructors"

        public MTConnectAgent(
            string uuid = null,
            long instanceId = 0,
            long deviceModelChangeTime = 0,
            bool initializeAgentDevice = true
            )
        {
            _uuid = !string.IsNullOrEmpty(uuid) ? uuid : Guid.NewGuid().ToString();
            _instanceId = instanceId > 0 ? instanceId : CreateInstanceId();
            _configuration = new AgentConfiguration();
            _information = new MTConnectAgentInformation(_uuid, _instanceId, _deviceModelChangeTime);
            _deviceModelChangeTime = deviceModelChangeTime;
            _mtconnectVersion = MTConnectVersions.Max;
            _version = GetAgentVersion();
            InitializeAgentDevice(initializeAgentDevice);
        }

        public MTConnectAgent(
            IAgentConfiguration configuration,
            string uuid = null,
            long instanceId = 0,
            long deviceModelChangeTime = 0,
            bool initializeAgentDevice = true
            )
        {
            _uuid = !string.IsNullOrEmpty(uuid) ? uuid : Guid.NewGuid().ToString();
            _instanceId = instanceId > 0 ? instanceId : CreateInstanceId();
            _configuration = configuration != null ? configuration : new AgentConfiguration();
            _information = new MTConnectAgentInformation(_uuid, _instanceId, _deviceModelChangeTime);
            _deviceModelChangeTime = deviceModelChangeTime;
            _mtconnectVersion = _configuration != null && _configuration.DefaultVersion != null ? _configuration.DefaultVersion : MTConnectVersions.Max;
            _version = GetAgentVersion();
            InitializeAgentDevice(initializeAgentDevice);
        }


        /// <summary>
        /// Start the MTConnect Agent
        /// </summary>
        public void Start()
        {
            if (_configuration.EnableMetrics)
            {
                _metrics = new MTConnectAgentMetrics(TimeSpan.FromSeconds(10), TimeSpan.FromMinutes(1));
                _metrics.DeviceMetricsUpdated += DeviceMetricsUpdated;
            }

            StartAgentInformationUpdateTimer();
            GarbageCollector.Initialize();
        }

        /// <summary>
        /// Stop the MTConnect Agent
        /// </summary>
        public void Stop()
        {
            if (_metrics != null) _metrics.Dispose();
            StopAgentInformationUpdateTimer();
        }

        public void Dispose()
        {
            Stop();
            GarbageCollector.DisposeInstance();
        }

        #endregion

        #region "Initialization"

        protected void InitializeAgentDevice(bool initializeDataItems = true)
        {
            _agent = new Agent(this);
            _agent.InitializeDataItems();
            _agent.Hash = _agent.GenerateHash();

            // Add Name and UUID to DeviceKey dictionary
            _deviceKeys.TryRemove(_agent.Name.ToLower(), out _);
            _deviceKeys.TryAdd(_agent.Name.ToLower(), _agent.Uuid);
            _deviceKeys.TryRemove(_agent.Uuid, out _);
            _deviceKeys.TryAdd(_agent.Uuid, _agent.Uuid);

            // Update Cached DataItems
            _deviceDataItems.TryRemove(_agent.Uuid, out _);
            _deviceDataItems.TryAdd(_agent.Uuid, _agent.GetDataItems());
            _deviceDataItemIds.TryRemove(_agent.Uuid, out _);
            _deviceDataItemIds.TryAdd(_agent.Uuid, _agent.GetDataItems().Select(o => o.Id));

            if (initializeDataItems)
            {
                _agent.InitializeObservations();
            }
        }

        internal void UpdateAgentDevice()
        {
            // Update Cached DataItems
            _deviceDataItems.TryRemove(_agent.Uuid, out _);
            _deviceDataItems.TryAdd(_agent.Uuid, _agent.GetDataItems());
            _deviceDataItemIds.TryRemove(_agent.Uuid, out _);
            _deviceDataItemIds.TryAdd(_agent.Uuid, _agent.GetDataItems().Select(o => o.Id));
        }

        #endregion

        #region "Entities"

        public string GetDeviceUuid(string deviceKey)
        {
            if (!string.IsNullOrEmpty(deviceKey))
            {
                // Lookup Device Uuid or Name
                if (_deviceKeys.TryGetValue(deviceKey, out var deviceUuid))
                {
                    return deviceUuid;
                }

                // Check for Case-Insensitive Device Name
                else if (_deviceKeys.TryGetValue(deviceKey.ToLower(), out deviceUuid))
                {
                    return deviceUuid;
                }
            }

            return null;
        }

        protected List<IDevice> ProcessDevices(IEnumerable<IDevice> devices, Version mtconnectVersion = null)
        {
            var objs = new List<IDevice>();

            if (!devices.IsNullOrEmpty())
            {
                foreach (var device in devices)
                {
                    var processedDevice = Device.Process(device, mtconnectVersion != null ? mtconnectVersion : MTConnectVersion);
                    if (processedDevice != null) objs.Add(processedDevice);
                }
            }

            return objs;
        }


        public IDevice GetDevice(string deviceKey, Version mtconnectVersion = null)
        {
            var deviceUuid = GetDeviceUuid(deviceKey);
            if (deviceUuid != null)
            {
                _devices.TryGetValue(deviceUuid, out var device);
                if (device != null)
                {
                    return Device.Process(device, mtconnectVersion != null ? mtconnectVersion : MTConnectVersion);
                }
            }

            return null;
        }

        public IEnumerable<IDevice> GetDevices(Version mtconnectVersion = null)
        {
            var allDevices = new List<IDevice>();
            allDevices.Add(_agent);
            var devices = _devices.Select(o => o.Value).ToList();
            if (!devices.IsNullOrEmpty()) allDevices.AddRange(devices);

            if (!allDevices.IsNullOrEmpty())
            {
                return ProcessDevices(allDevices, mtconnectVersion);
            }

            return null;
        }

        public IEnumerable<IDevice> GetDevices(string deviceType, Version mtconnectVersion = null)
        {
            var allDevices = new List<IDevice>();
            if (string.IsNullOrEmpty(deviceType) || deviceType.ToLower() == "agent") allDevices.Add(Agent);
            var devices = _devices.Select(o => o.Value).ToList();
            if (!devices.IsNullOrEmpty()) allDevices.AddRange(devices);

            if (!allDevices.IsNullOrEmpty())
            {
                return ProcessDevices(allDevices, mtconnectVersion);
            }

            return null;
        }


        public IDataItem GetDataItem(string deviceKey, string dataItemKey)
        {
            if (!string.IsNullOrEmpty(deviceKey) && !string.IsNullOrEmpty(dataItemKey))
            {
                // Lookup DeviceUuid from deviceKey
                var deviceUuid = GetDeviceUuid(deviceKey);
                if (!string.IsNullOrEmpty(deviceUuid))
                {
                    var lookupKey = deviceUuid + ":" + dataItemKey;

                    // Lookup DataItemId from DeviceUuid:DataItemKey
                    if (_dataItemKeys.TryGetValue(lookupKey, out var dataItemId))
                    {
                        // Lookup DataItem from cached list
                        if (_dataItems.TryGetValue(dataItemId, out var dataItem))
                        {
                            return dataItem;
                        }
                    }
                    else
                    {
                        // Get Device From DeviceBuffer
                        _devices.TryGetValue(deviceKey, out var device);
                        if (device == null && deviceUuid == _uuid) device = _agent;

                        // Get DataItem from Device DataItemKey
                        var dataItem = GetDataItemFromKey(device, dataItemKey);
                        if (dataItem != null)
                        {
                            // Update DataItem in cached list
                            if (!_dataItems.ContainsKey(dataItem.Id))
                            {
                                _dataItems.TryAdd(dataItem.Id, dataItem);
                            }

                            // Update DataItemKey in cached list
                            _dataItemKeys.TryAdd(lookupKey, dataItem.Id);

                            return dataItem;
                        }
                    }
                }
            }

            return null;
        }

        private IDataItem GetDataItemFromKey(IDevice device, string key)
        {
            if (device != null)
            {
                // Get list of all DataItems for Device
                var dataItems = device.GetDataItems();
                if (!dataItems.IsNullOrEmpty())
                {
                    // Check DataItem ID
                    var dataItem = dataItems.FirstOrDefault(o => o.Id == key);

                    // Check DataItem Name
                    if (dataItem == null) dataItem = dataItems.FirstOrDefault(o => o.Name == key);

                    // Check DataItem Source DataItemId
                    if (dataItem == null) dataItem = dataItems.FirstOrDefault(o => o.Source != null && o.Source.DataItemId == key);

                    // Check DataItem Source Value
                    if (dataItem == null) dataItem = dataItems.FirstOrDefault(o => o.Source != null && o.Source.Value == key);

                    // Return DataItem
                    return dataItem;
                }
            }

            return null;
        }

        public IEnumerable<IDataItem> GetDataItems(string deviceKey)
        {
            if (!string.IsNullOrEmpty(deviceKey))
            {
                // Lookup DeviceUuid from deviceKey
                var deviceUuid = GetDeviceUuid(deviceKey);
                if (!string.IsNullOrEmpty(deviceUuid))
                {
                    if (_deviceDataItems.TryGetValue(deviceUuid, out var dataItems))
                    {
                        return dataItems;
                    }
                }
            }

            return null;
        }


        public IEnumerable<IObservationOutput> GetCurrentObservations(Version mtconnectVersion = null)
        {
            var observations = new List<IObservationOutput>();

            var version = mtconnectVersion != null ? mtconnectVersion : MTConnectVersion;

            var allDevices = new List<IDevice>();
            allDevices.Add(_agent);
            var devices = GetDevices(version);
            if (!devices.IsNullOrEmpty()) allDevices.AddRange(devices);

            if (!allDevices.IsNullOrEmpty())
            {
                foreach (var device in allDevices)
                {
                    observations.AddRange(GetCurrentObservations(device.Uuid, version));
                }
            }

            return observations;
        }

        public IEnumerable<IObservationOutput> GetCurrentObservations(string deviceKey, Version mtconnectVersion = null)
        {
            var observations = new List<IObservationOutput>();

            var version = mtconnectVersion != null ? mtconnectVersion : MTConnectVersion;

            var device = GetDevice(deviceKey, version);
            if (device != null)
            {
                var dataItems = GetDataItems(device.Uuid);
                if (!dataItems.IsNullOrEmpty())
                {
                    foreach (var dataItem in dataItems)
                    {
                        var hash = $"{device.Uuid}:{dataItem.Id}";

                        if (dataItem.Category == DataItemCategory.CONDITION)
                        {
                            if (_currentConditions.TryGetValue(hash, out var observationInputs))
                            {
                                if (!observationInputs.IsNullOrEmpty())
                                {
                                    foreach (var observationInput in observationInputs)
                                    {
                                        observations.Add(CreateObservation(dataItem, observationInput));
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (_currentObservations.TryGetValue(hash, out var observationInput))
                            {
                                observations.Add(CreateObservation(dataItem, observationInput));
                            }
                        }
                    }
                }
            }

            return observations;
        }

        public IEnumerable<IObservationOutput> GetCurrentObservations(string deviceKey, string dataItemKey, Version mtconnectVersion = null)
        {
            var observations = new List<IObservationOutput>();

            var version = mtconnectVersion != null ? mtconnectVersion : MTConnectVersion;

            var device = GetDevice(deviceKey, version);
            if (device != null)
            {
                var dataItem = GetDataItem(device.Uuid, dataItemKey);
                if (dataItem != null)
                {
                    var hash = $"{device.Uuid}:{dataItem.Id}";

                    if (dataItem.Category == DataItemCategory.CONDITION)
                    {
                        if (_currentConditions.TryGetValue(hash, out var observationInputs))
                        {
                            if (!observationInputs.IsNullOrEmpty())
                            {
                                foreach (var observationInput in observationInputs)
                                {
                                    observations.Add(CreateObservation(dataItem, observationInput));
                                }
                            }
                        }
                    }
                    else
                    {
                        if (_currentObservations.TryGetValue(hash, out var observationInput))
                        {
                            observations.Add(CreateObservation(dataItem, observationInput));
                        }
                    }
                }
            }

            return observations;
        }

        private IObservationOutput CreateObservation(IDataItem dataItem, IObservationInput observationInput)
        {
            var observation = new ObservationOutput();
            observation._dataItem = dataItem;
            if (dataItem.Device != null) observation._deviceUuid = dataItem.Device.Uuid;
            observation._dataItemId = dataItem.Id;
            observation._category = dataItem.Category;
            observation._representation = dataItem.Representation;
            observation._type = dataItem.Type;
            observation._subType = dataItem.SubType;
            observation._name = dataItem.Name;
            observation._compositionId = dataItem.CompositionId;
            observation._instanceId = _instanceId;
            //observation._sequence = observationInput.Sequence; // No Sequence in IObservationInput
            observation._timestamp = observationInput.Timestamp.ToDateTime();
            observation._values = observationInput.Values?.ToArray();
            return observation;
        }


        public virtual IEnumerable<IAsset> GetAssets(Version mtconnectVersion = null)
        {
            return null;
        }

        public virtual IEnumerable<IAsset> GetAssets(string deviceKey, Version mtconnectVersion = null)
        {
            return null;
        }

        /// <summary>
        /// Remove the Asset with the specified Asset ID
        /// </summary>
        /// <param name="assetId">The ID of the Asset to remove</param>
        /// <param name="timestamp">The Timestamp of when the Asset was removed in Unix Ticks (1/10,000 of a millisecond)</param>
        /// <returns>Returns True if the Asset was successfully removed</returns>
        public virtual bool RemoveAsset(string assetId, long timestamp = 0)
        {
            return false;
        }

        /// <summary>
        /// Remove the Asset with the specified Asset ID
        /// </summary>
        /// <param name="assetId">The ID of the Asset to remove</param>
        /// <param name="timestamp">The Timestamp of when the Asset was removed</param>
        /// <returns>Returns True if the Asset was successfully removed</returns>
        public virtual bool RemoveAsset(string assetId, DateTime timestamp)
        {
            return false;
        }


        /// <summary>
        /// Remove all Assets with the specified Type
        /// </summary>
        /// <param name="assetType">The Type of the Assets to remove</param>
        /// <param name="timestamp">The Timestamp of when the Assets were removed in Unix Ticks (1/10,000 of a millisecond)</param>
        /// <returns>Returns True if the Assets were successfully removed</returns>
        public virtual bool RemoveAllAssets(string assetType, long timestamp = 0)
        {
            return false;
        }

        /// <summary>
        /// Remove all Assets with the specified Type
        /// </summary>
        /// <param name="assetType">The Type of the Assets to remove</param>
        /// <param name="timestamp">The Timestamp of when the Assets were removed</param>
        /// <returns>Returns True if the Assets were successfully removed</returns>
        public virtual bool RemoveAllAssets(string assetType, DateTime timestamp)
        {
            return false;
        }

        #endregion

        #region "Add"

        #region "Internal"

        public virtual void InitializeDataItems(IDevice device, long timestamp = 0)
        {
            if (device != null)
            {
                // Get All DataItems for the Device
                var dataItems = device.GetDataItems();
                if (!dataItems.IsNullOrEmpty())
                {
                    var ts = timestamp > 0 ? timestamp : UnixDateTime.Now;

                    foreach (var dataItem in dataItems)
                    {
                        // Add Unavailable Observation to ObservationBuffer
                        var observation = new ObservationInput();
                        observation.DeviceKey = device.Uuid;
                        observation.DataItemKey = dataItem.Id;
                        observation.Timestamp = ts;

                        var values = new List<ObservationValue>();

                        var valueType = dataItem.Category == DataItemCategory.CONDITION ? ValueKeys.Level : ValueKeys.Result;
                        var value = !string.IsNullOrEmpty(dataItem.InitialValue) ? dataItem.InitialValue : Observation.Unavailable;
                        values.Add(new ObservationValue(valueType, value));

                        // Add Required Values
                        switch (dataItem.Representation)
                        {
                            case DataItemRepresentation.DATA_SET: values.Add(new ObservationValue(ValueKeys.Count, 0)); break;
                            case DataItemRepresentation.TABLE: values.Add(new ObservationValue(ValueKeys.Count, 0)); break;
                            case DataItemRepresentation.TIME_SERIES: values.Add(new ObservationValue(ValueKeys.SampleCount, 0)); break;
                        }

                        observation.Values = values;

                        AddObservation(device.Uuid, observation);
                    }
                }
            }
        }

        protected bool UpdateCurrentObservation(string deviceUuid, IDataItem dataItem, IObservationInput observation)
        {
            if (_currentObservations != null && observation != null && !string.IsNullOrEmpty(deviceUuid) && dataItem != null)
            {
                var hash = $"{deviceUuid}:{dataItem.Id}";

                _currentObservations.TryGetValue(hash, out var existingObservation);
                if (observation != null && existingObservation != null)
                {
                    // Check Filters
                    var update = FilterPeriod(dataItem, observation.Timestamp, existingObservation.Timestamp);
                    if (update) update = FilterDelta(dataItem, observation, existingObservation);

                    // Update if Filters are passed or if the DataItem is set to Discrete
                    if (update || dataItem.Discrete)
                    {
                        _currentObservations.TryRemove(hash, out var _);
                        return _currentObservations.TryAdd(hash, observation);
                    }
                }
                else
                {
                    _currentObservations.TryRemove(hash, out var _);
                    return _currentObservations.TryAdd(hash, observation);
                }
            }

            return false;
        }

        protected bool UpdateCurrentCondition(string deviceUuid, IDataItem dataItem, IObservationInput observation)
        {
            if (_currentConditions != null && observation != null && !string.IsNullOrEmpty(deviceUuid) && dataItem != null)
            {
                var observations = new List<IObservationInput>();

                // Get Existing Condition Observations for DataItem
                var hash = $"{deviceUuid}:{dataItem.Id}";

                _currentConditions.TryGetValue(hash, out var existingObservations);

                if (existingObservations.IsNullOrEmpty() || !existingObservations.Any(o => o.ChangeId == observation.ChangeId))
                {
                    observations.Add(observation);
                }

                // Add previous Condition Observations (if new Condition is not NORMAL or UNAVAILABLE)
                byte[] existingHash = null;
                if (observation != null && !existingObservations.IsNullOrEmpty())
                {
                    existingHash = StringFunctions.ToMD5HashBytes(existingObservations.Select(o => o.ChangeId).ToArray());

                    var conditionLevel = observation.GetValue(ValueKeys.Level);
                    var nativeCode = observation.GetValue(ValueKeys.NativeCode);

                    if (!(conditionLevel == ConditionLevel.NORMAL.ToString() && string.IsNullOrEmpty(nativeCode)) &&
                        conditionLevel != ConditionLevel.UNAVAILABLE.ToString())
                    {
                        var i = 0;
                        foreach (var existingObservation in existingObservations)
                        {
                            var existingLevel = existingObservation.GetValue(ValueKeys.Level);
                            var existingNativeCode = existingObservation.GetValue(ValueKeys.NativeCode);

                            if (!string.IsNullOrEmpty(existingNativeCode) && existingNativeCode != nativeCode && existingLevel != ConditionLevel.UNAVAILABLE.ToString())
                            {
                                observations.Insert(i, existingObservation);
                                i++;
                            }
                        }
                    }
                }

                // Compare Hashes. If different, then update current list
                byte[] newHash = StringFunctions.ToMD5HashBytes(observations.Select(o => o.ChangeId).ToArray());
                if (!ObjectExtensions.ByteArraysEqual(existingHash, newHash))
                {
                    _currentConditions.TryRemove(hash, out var _);
                    return _currentConditions.TryAdd(hash, observations);
                }
            }

            return false;
        }


        private static bool FilterPeriod(IDataItem dataItem, long newTimestamp, long existingTimestamp)
        {
            if (dataItem != null)
            {
                if (!dataItem.Filters.IsNullOrEmpty())
                {
                    foreach (var filter in dataItem.Filters)
                    {
                        if (filter.Type == DataItemFilterType.PERIOD)
                        {
                            if (filter.Value > 0)
                            {
                                // Get Period based on Seconds specified in Filter
                                var period = TimeSpan.FromSeconds(filter.Value);

                                // Get Duration between newTimestamp and existingTimestamp
                                var duration = TimeSpan.FromTicks(newTimestamp - existingTimestamp);

                                return duration > period;
                            }
                        }
                    }
                }

                return true;
            }

            return false;
        }

        private static bool FilterDelta(IDataItem dataItem, IObservationInput newObservation, IObservationInput existingObservation)
        {
            if (dataItem != null)
            {
                if (!ObjectExtensions.ByteArraysEqual(newObservation.ChangeId, existingObservation.ChangeId))
                {
                    if (!dataItem.Filters.IsNullOrEmpty() && dataItem.Representation == DataItemRepresentation.VALUE)
                    {
                        foreach (var filter in dataItem.Filters)
                        {
                            if (filter.Type == DataItemFilterType.MINIMUM_DELTA)
                            {
                                if (filter.Value > 0)
                                {
                                    var x = newObservation.GetValue(ValueKeys.Result).ToDouble();
                                    var y = existingObservation.GetValue(ValueKeys.Result).ToDouble();

                                    // If difference between New and Existing exceeds Filter Minimum Delta Value
                                    return Math.Abs(x - y) > filter.Value;
                                }
                            }
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        #endregion


        #region "Devices"

        #region "Internal"

        private Device NormalizeDevice(IDevice device)
        {
            if (device != null)
            {
                Device obj = null;

                if (device.Type == Device.TypeId) obj = new Device();
                else if (device.Type == Agent.TypeId) obj = new Agent();

                if (obj != null)
                {
                    obj.Id = device.Id;
                    obj.Name = device.Name;
                    obj.NativeName = device.NativeName;
                    obj.Uuid = device.Uuid;
                    obj.InstanceId = _instanceId;
                    obj.Type = device.Type;
                    obj.SampleRate = device.SampleRate;
                    obj.SampleInterval = device.SampleInterval;
                    obj.Iso841Class = device.Iso841Class;
                    obj.CoordinateSystemIdRef = device.CoordinateSystemIdRef;
                    obj.MTConnectVersion = device.MTConnectVersion;
                    obj.Configuration = device.Configuration;
                    obj.References = device.References;
                    obj.Description = NormalizeDescription(device.Description);
                    obj.DataItems = NormalizeDataItems(device.DataItems, obj, obj);
                    obj.Compositions = NormalizeCompositions(device.Compositions, obj, obj);
                    obj.Components = NormalizeComponents(device.Components, obj, obj);

                    // Add Required Availability DataItem
                    if (obj.DataItems.IsNullOrEmpty() || !obj.DataItems.Any(o => o.Type == AvailabilityDataItem.TypeId))
                    {
                        var availability = new AvailabilityDataItem(obj.Id);
                        availability.Device = obj;
                        availability.Container = obj;
                        availability.Name = AvailabilityDataItem.NameId;
                        var x = obj.DataItems.ToList();
                        x.Add(availability);
                        obj.DataItems = x;
                    }

                    // Add Required AssetChanged DataItem
                    if (obj.DataItems.IsNullOrEmpty() || !obj.DataItems.Any(o => o.Type == AssetChangedDataItem.TypeId))
                    {
                        var assetChanged = new AssetChangedDataItem(obj.Id);
                        assetChanged.Device = obj;
                        assetChanged.Container = obj;
                        assetChanged.Name = AssetChangedDataItem.NameId;
                        var x = obj.DataItems.ToList();
                        x.Add(assetChanged);
                        obj.DataItems = x;
                    }

                    // Add Required AssetRemoved DataItem
                    if (obj.DataItems.IsNullOrEmpty() || !obj.DataItems.Any(o => o.Type == AssetRemovedDataItem.TypeId))
                    {
                        var assetRemoved = new AssetRemovedDataItem(obj.Id);
                        assetRemoved.Device = obj;
                        assetRemoved.Container = obj;
                        assetRemoved.Name = AssetRemovedDataItem.NameId;
                        var x = obj.DataItems.ToList();
                        x.Add(assetRemoved);
                        obj.DataItems = x;
                    }

                    // Add Required AssetCount DataItem
                    if (obj.DataItems.IsNullOrEmpty() || !obj.DataItems.Any(o => o.Type == AssetCountDataItem.TypeId))
                    {
                        var assetcount = new AssetCountDataItem(obj.Id);
                        assetcount.Device = obj;
                        assetcount.Container = obj;
                        assetcount.Name = AssetCountDataItem.NameId;
                        var x = obj.DataItems.ToList();
                        x.Add(assetcount);
                        obj.DataItems = x;
                    }


                    // Generic Components
                    var genericComponents = obj.GetComponents()?.Where(o => o.GetType() == typeof(Component));
                    if (!genericComponents.IsNullOrEmpty())
                    {
                        foreach (var genericComponent in genericComponents)
                        {
                            var validationResults = new ValidationResult(false, $"Invalid Component : \"{genericComponent.Type}\" Not Found");
                            if (_configuration.InputValidationLevel > InputValidationLevel.Ignore)
                            {
                                if (InvalidComponentAdded != null) InvalidComponentAdded.Invoke(obj.Uuid, genericComponent, validationResults);

                                // Remove Component from Device
                                if (_configuration.InputValidationLevel == InputValidationLevel.Remove) obj.RemoveComponent(genericComponent.Id);

                                // Invalidate entire Device
                                if (_configuration.InputValidationLevel == InputValidationLevel.Strict) return null;
                            }
                        }
                    }

                    // Generic Compositions
                    var genericCompositions = obj.GetCompositions()?.Where(o => o.GetType() == typeof(Composition));
                    if (!genericCompositions.IsNullOrEmpty())
                    {
                        foreach (var genericComposition in genericCompositions)
                        {
                            var validationResults = new ValidationResult(false, $"Invalid Composition : \"{genericComposition.Type}\" Not Found");
                            if (_configuration.InputValidationLevel > InputValidationLevel.Ignore)
                            {
                                if (InvalidCompositionAdded != null) InvalidCompositionAdded.Invoke(obj.Uuid, genericComposition, validationResults);

                                // Remove Compsition from Device
                                if (_configuration.InputValidationLevel == InputValidationLevel.Remove) obj.RemoveComposition(genericComposition.Id);

                                // Invalidate entire Device
                                if (_configuration.InputValidationLevel == InputValidationLevel.Strict) return null;
                            }
                        }
                    }

                    // Generic DataItems
                    var genericDataItems = obj.GetDataItems()?.Where(o => o.GetType() == typeof(DataItem));
                    if (!genericDataItems.IsNullOrEmpty())
                    {
                        foreach (var genericDataItem in genericDataItems)
                        {
                            var validationResults = new ValidationResult(false, $"Invalid DataItem : \"{genericDataItem.Type}\" Not Found");
                            if (_configuration.InputValidationLevel > InputValidationLevel.Ignore)
                            {
                                if (InvalidDataItemAdded != null) InvalidDataItemAdded.Invoke(obj.Uuid, genericDataItem, validationResults);

                                // Remove DataItem from Device
                                if (_configuration.InputValidationLevel == InputValidationLevel.Remove) obj.RemoveDataItem(genericDataItem.Id);

                                // Invalidate entire Device
                                if (_configuration.InputValidationLevel == InputValidationLevel.Strict) return null;
                            }
                        }
                    }

                    // Generate Device Hash
                    obj.Hash = obj.GenerateHash();
 
                    return obj;
                } 
            }

            return null;
        }

        private List<IComponent> NormalizeComponents(IEnumerable<IComponent> components, IContainer parent, IDevice device)
        {
            if (!components.IsNullOrEmpty())
            {
                var objs = new List<IComponent>();

                foreach (var component in components)
                {
                    var obj = Component.Create(component.Type);
                    obj.Id = component.Id;
                    obj.Uuid = component.Uuid;
                    obj.Name = component.Name;
                    obj.NativeName = component.NativeName;
                    obj.Type = component.Type;
                    obj.Description = component.Description;
                    obj.SampleRate = component.SampleRate;
                    obj.SampleInterval = component.SampleInterval;
                    obj.References = component.References;
                    obj.Configuration = component.Configuration;
                    obj.Parent = parent;

                    obj.Components = NormalizeComponents(component.Components, obj, device);
                    obj.Compositions = NormalizeCompositions(component.Compositions, obj, device);
                    obj.DataItems = NormalizeDataItems(component.DataItems, obj, device);

                    objs.Add(obj);
                }

                return objs;
            }

            return new List<IComponent>();
        }

        private List<IComposition> NormalizeCompositions(IEnumerable<IComposition> compositions, IContainer parent, IDevice device)
        {
            if (!compositions.IsNullOrEmpty())
            {
                var objs = new List<IComposition>();

                foreach (var composition in compositions)
                {
                    var obj = Composition.Create(composition.Type);
                    obj.Id = composition.Id;
                    obj.Uuid = composition.Uuid;
                    obj.Name = composition.Name;
                    obj.NativeName = composition.NativeName;
                    obj.Type = composition.Type;
                    obj.Description = composition.Description;
                    obj.SampleRate = composition.SampleRate;
                    obj.SampleInterval = composition.SampleInterval;
                    obj.References = composition.References;
                    obj.Configuration = composition.Configuration;
                    obj.Parent = parent;

                    obj.DataItems = NormalizeDataItems(composition.DataItems, obj, device);

                    objs.Add(obj);
                }

                return objs;
            }

            return new List<IComposition>();
        }

        private List<IDataItem> NormalizeDataItems(IEnumerable<IDataItem> dataItems, IContainer parent, IDevice device)
        {
            if (!dataItems.IsNullOrEmpty())
            {
                var objs = new List<IDataItem>();

                foreach (var dataItem in dataItems)
                {
                    var obj = DataItem.Create(dataItem);
                    obj.Category = dataItem.Category;
                    obj.Id = dataItem.Id;
                    obj.Name = dataItem.Name;
                    obj.Type = dataItem.Type;
                    obj.SubType = dataItem.SubType;
                    obj.NativeUnits = dataItem.NativeUnits;
                    obj.NativeScale = dataItem.NativeScale;
                    obj.SampleRate = dataItem.SampleRate;
                    obj.Source = dataItem.Source;
                    obj.CompositionId = dataItem.CompositionId;
                    obj.Relationships = dataItem.Relationships;
                    obj.Representation = dataItem.Representation;
                    obj.ResetTrigger = dataItem.ResetTrigger;
                    obj.CoordinateSystem = dataItem.CoordinateSystem;
                    obj.Constraints = dataItem.Constraints;
                    obj.Definition = dataItem.Definition;
                    obj.Units = dataItem.Units;
                    obj.Statistic = dataItem.Statistic;
                    obj.SignificantDigits = dataItem.SignificantDigits;
                    obj.Filters = dataItem.Filters;
                    obj.InitialValue = dataItem.InitialValue;
                    obj.Discrete = dataItem.Discrete;
                    obj.Device = device;
                    obj.Container = parent;

                    objs.Add(obj);
                }

                return objs;
            }

            return new List<IDataItem>();
        }

        private Description NormalizeDescription(IDescription description)
        {
            if (description != null)
            {
                var obj = new Description();
                obj.Manufacturer = !string.IsNullOrEmpty(description.Manufacturer) ? description.Manufacturer.Trim() : null;
                obj.Model = !string.IsNullOrEmpty(description.Model) ? description.Model.Trim() : null;
                obj.SerialNumber = !string.IsNullOrEmpty(description.SerialNumber) ? description.SerialNumber.Trim() : null;
                obj.Station = !string.IsNullOrEmpty(description.Station) ? description.Station.Trim() : null;
                obj.Value = !string.IsNullOrEmpty(description.Value) ? description.Value.Trim() : null;
                return obj;
            }

            return null;
        }



        private bool AddDeviceAddedObservation(IDevice device, long timestamp = 0)
        {
            if (_agent != null && device != null)
            {
                var dataItems = _agent.GetDataItems();
                if (!dataItems.IsNullOrEmpty())
                {
                    var dataItem = dataItems.FirstOrDefault(o => o.Type == DeviceAddedDataItem.TypeId);
                    if (dataItem != null)
                    {
                        // Create new Observation
                        var observation = Observation.Create(dataItem);
                        observation.DeviceUuid = _agent.Uuid;
                        observation.InstanceId = _instanceId;
                        observation.Timestamp = timestamp.ToDateTime();
                        observation.AddValues(new List<ObservationValue>
                        {
                            new ObservationValue(ValueKeys.Result, device.Uuid)
                        });

                        ObservationAdded?.Invoke(this, observation);

                        return true;
                    }
                }
            }

            return false;
        }


        private bool AddDeviceChangedObservation(IDevice device, long timestamp = 0)
        {
            if (_agent != null && device != null)
            {
                var dataItems = _agent.GetDataItems();
                if (!dataItems.IsNullOrEmpty())
                {
                    var dataItem = dataItems.FirstOrDefault(o => o.Type == DeviceChangedDataItem.TypeId);
                    if (dataItem != null)
                    {
                        // Create new Observation
                        var observation = Observation.Create(dataItem);
                        observation.DeviceUuid = _agent.Uuid;
                        observation.InstanceId = _instanceId;
                        observation.Timestamp = timestamp.ToDateTime();
                        observation.AddValues(new List<ObservationValue>
                        {
                            new ObservationValue(ValueKeys.Result, device.Uuid)
                        });

                        ObservationAdded?.Invoke(this, observation);

                        return true;
                    }
                }
            }

            return false;
        }

        private bool AddDeviceRemovedObservation(IDevice device, long timestamp = 0)
        {
            if (_agent != null && device != null)
            {
                var dataItems = _agent.GetDataItems();
                if (!dataItems.IsNullOrEmpty())
                {
                    var dataItem = dataItems.FirstOrDefault(o => o.Type == DeviceRemovedDataItem.TypeId);
                    if (dataItem != null)
                    {
                        // Create new Observation
                        var observation = Observation.Create(dataItem);
                        observation.DeviceUuid = _agent.Uuid;
                        observation.InstanceId = _instanceId;
                        observation.Timestamp = timestamp.ToDateTime();
                        observation.AddValues(new List<ObservationValue>
                        {
                            new ObservationValue(ValueKeys.Result, device.Uuid)
                        });

                        ObservationAdded?.Invoke(this, observation);

                        return true;
                    }
                }
            }

            return false;
        }

        #endregion


        /// <summary>
        /// Add a new MTConnectDevice to the Agent's Buffer
        /// </summary>
        public bool AddDevice(IDevice device, bool initializeDataItems = true)
        {
            if (device != null)
            {
                // Create new object (to validate and prevent derived classes that won't serialize right with XML)
                var obj = NormalizeDevice(device);
                if (obj != null)
                {
                    // Get Existing Device (if exists)
                    _devices.TryGetValue(obj.Uuid, out var existingDevice);

                    // Check if Device Already Exists in the Device Buffer and is changed
                    if (existingDevice != null && obj.Hash == existingDevice.Hash)
                    {
                        return true;
                    }

                    // Add the Device to the Buffer
                    var success = _devices.TryAdd(obj.Uuid, obj);
                    if (success)
                    {
                        // Add Name and UUID to DeviceKey dictionary
                        _deviceKeys.TryAdd(obj.Name.ToLower(), obj.Uuid);
                        _deviceKeys.TryAdd(obj.Uuid, obj.Uuid);

                        // Update Cached DataItems
                        _deviceDataItems.TryRemove(obj.Uuid, out _);
                        _deviceDataItems.TryAdd(obj.Uuid, obj.GetDataItems());
                        _deviceDataItemIds.TryRemove(obj.Uuid, out _);
                        _deviceDataItemIds.TryAdd(obj.Uuid, obj.GetDataItems().Select(o => o.Id));

                        if (initializeDataItems)
                        {
                            var timestamp = UnixDateTime.Now;

                            if (existingDevice != null)
                            {
                                AddDeviceChangedObservation(obj, timestamp);
                            }
                            else
                            {
                                AddDeviceAddedObservation(obj, timestamp);
                            }

                            InitializeDataItems(obj);

                            _deviceModelChangeTime = timestamp;
                            _updateInformation = true;
                        }

                        DeviceAdded?.Invoke(this, obj);
                    }

                    return success;
                }
            }

            return false;
        }

        /// <summary>
        /// Add new MTConnectDevices to the Agent's Buffer
        /// </summary>
        public bool AddDevices(IEnumerable<IDevice> devices, bool initializeDataItems = true)
        {
            if (!devices.IsNullOrEmpty())
            {
                bool success = false;

                foreach (var device in devices)
                {
                    success = AddDevice(device, initializeDataItems);
                    if (!success) break;
                }

                return success;
            }

            return false;
        }

        #endregion

        #region "Observations"

        #region "Internal"

        private IObservationInput ConvertObservationValue(IDataItem dataItem, IObservationInput observation)
        {
            if (dataItem != null && observation != null)
            {
                if (dataItem.Category == DataItemCategory.SAMPLE)
                {
                    // Get the Result Value
                    var result = observation.GetValue(ValueKeys.Result);
                    if (!string.IsNullOrEmpty(result) && result != Observation.Unavailable)
                    {
                        var units = dataItem.Units;
                        var nativeUnits = dataItem.NativeUnits;

                        if (!result.IsNumeric())
                        {
                            // Get the SampleValue for the DataItem Type
                            if (dataItem.Units == Units.DEGREE_3D)
                            {
                                // Remove the "_3D" suffix from the Units and NativeUnits
                                units = Remove3dSuffix(units);
                                nativeUnits = Remove3dSuffix(nativeUnits);

                                // Create a new Degree3D object to parse the Result
                                var degree3d = Degree3D.FromString(result);
                                if (degree3d != null)
                                {
                                    degree3d.A = Units.Convert(degree3d.A, units, nativeUnits);
                                    degree3d.B = Units.Convert(degree3d.B, units, nativeUnits);
                                    degree3d.C = Units.Convert(degree3d.C, units, nativeUnits);

                                    // Apply the NativeScale
                                    if (dataItem.NativeScale > 0)
                                    {
                                        degree3d.A = degree3d.A / dataItem.NativeScale;
                                        degree3d.B = degree3d.B / dataItem.NativeScale;
                                        degree3d.C = degree3d.C / dataItem.NativeScale;
                                    }

                                    // Convert _3D back to string using the appropriate format and set to Result
                                    result = degree3d.ToString();
                                }
                            }
                            else if (dataItem.Units == Units.MILLIMETER_3D || dataItem.Units == Units.UNIT_VECTOR_3D)
                            {
                                // Remove the "_3D" suffix from the Units and NativeUnits
                                units = Remove3dSuffix(units);
                                nativeUnits = Remove3dSuffix(nativeUnits);

                                // Create a new Position3D object to parse the Result
                                var position3d = Position3D.FromString(result);
                                if (position3d != null)
                                {
                                    position3d.X = Units.Convert(position3d.X, units, nativeUnits);
                                    position3d.Y = Units.Convert(position3d.Y, units, nativeUnits);
                                    position3d.Z = Units.Convert(position3d.Z, units, nativeUnits);

                                    // Apply the NativeScale
                                    if (dataItem.NativeScale > 0)
                                    {
                                        position3d.X = position3d.X / dataItem.NativeScale;
                                        position3d.Y = position3d.Y / dataItem.NativeScale;
                                        position3d.Z = position3d.Z / dataItem.NativeScale;
                                    }

                                    // Convert _3D back to string using the appropriate format and set Result
                                    result = position3d.ToString();
                                }
                            }
                        }
                        else
                        {
                            // Directly convert the Units if no SampleValue class is found
                            var value = Units.Convert(result.ToDouble(), units, nativeUnits);

                            // Apply the NativeScale
                            if (dataItem.NativeScale > 0) value = value / dataItem.NativeScale;

                            // Set Result to value
                            result = value.ToString();
                        }

                        // Replace the Result value in the Observation
                        observation.AddValue(ValueKeys.Result, result);
                    }
                }
            }

            return observation;
        }

        private string Remove3dSuffix(string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                var i = s.IndexOf("_3D");
                if (i >= 0)
                {
                    s = s.Substring(0, i);
                }
            }
            return s;
        }

        #endregion


        /// <summary>
        /// Add a new Observation to the Agent for the specified Device and DataItem
        /// </summary>
        /// <param name="deviceKey">The (Name or Uuid) of the Device</param>
        /// <param name="dataItemKey">The (Name, ID, or Source) of the DataItem</param>
        /// <param name="value">The Value of the Observation (equivalent to ValueKey = Value)</param>
        /// <param name="convertUnits">Used to override the default configuration for the Agent to ConvertUnits</param>
        /// <param name="ignoreCase">Used to override the default configuration for the Agent to IgnoreCase of the Value</param>
        /// <returns>True if the Observation was added successfully</returns>
        public bool AddObservation(string deviceKey, string dataItemKey, object value, bool? convertUnits = null, bool? ignoreCase = null)
        {
            return AddObservation(deviceKey, new ObservationInput
            {
                DeviceKey = deviceKey,
                DataItemKey = dataItemKey,
                Values = new List<ObservationValue> { new ObservationValue(ValueKeys.Result, value) },
                Timestamp = UnixDateTime.Now
            });
        }

        /// <summary>
        /// Add a new Observation to the Agent for the specified Device and DataItem
        /// </summary>
        /// <param name="deviceKey">The (Name or Uuid) of the Device</param>
        /// <param name="dataItemKey">The (Name, ID, or Source) of the DataItem</param>
        /// <param name="value">The Value of the Observation (equivalent to ValueKey = Value)</param>
        /// <param name="timestamp">The Timestamp of the Observation in Unix Ticks (1/10,000 of a millisecond)</param>
        /// <param name="convertUnits">Used to override the default configuration for the Agent to ConvertUnits</param>
        /// <param name="ignoreCase">Used to override the default configuration for the Agent to IgnoreCase of the Value</param>
        /// <returns>True if the Observation was added successfully</returns>
        public bool AddObservation(string deviceKey, string dataItemKey, object value, long timestamp, bool? convertUnits = null, bool? ignoreCase = null)
        {
            return AddObservation(deviceKey, new ObservationInput
            {
                DeviceKey = deviceKey,
                DataItemKey = dataItemKey,
                Values = new List<ObservationValue> { new ObservationValue(ValueKeys.Result, value) },
                Timestamp = timestamp
            });
        }

        /// <summary>
        /// Add a new Observation to the Agent for the specified Device and DataItem
        /// </summary>
        /// <param name="deviceKey">The (Name or Uuid) of the Device</param>
        /// <param name="dataItemKey">The (Name, ID, or Source) of the DataItem</param>
        /// <param name="value">The Value of the Observation (equivalent to ValueKey = Value)</param>
        /// <param name="timestamp">The Timestamp of the Observation</param>
        /// <param name="convertUnits">Used to override the default configuration for the Agent to ConvertUnits</param>
        /// <param name="ignoreCase">Used to override the default configuration for the Agent to IgnoreCase of the Value</param>
        /// <returns>True if the Observation was added successfully</returns>
        public bool AddObservation(string deviceKey, string dataItemKey, object value, DateTime timestamp, bool? convertUnits = null, bool? ignoreCase = null)
        {
            return AddObservation(deviceKey, new ObservationInput
            {
                DeviceKey = deviceKey,
                DataItemKey = dataItemKey,
                Values = new List<ObservationValue> { new ObservationValue(ValueKeys.Result, value) },
                Timestamp = timestamp.ToUnixTime()
            });
        }


        /// <summary>
        /// Add a new Observation to the Agent for the specified Device and DataItem
        /// </summary>
        /// <param name="deviceKey">The (Name or Uuid) of the Device</param>
        /// <param name="dataItemKey">The (Name, ID, or Source) of the DataItem</param>
        /// <param name="valueKey">The ValueKey to use for the Value parameter</param>
        /// <param name="value">The Value of the Observation</param>
        /// <param name="convertUnits">Used to override the default configuration for the Agent to ConvertUnits</param>
        /// <param name="ignoreCase">Used to override the default configuration for the Agent to IgnoreCase of the Value</param>
        /// <returns>True if the Observation was added successfully</returns>
        public bool AddObservation(string deviceKey, string dataItemKey, string valueKey, object value, bool? convertUnits = null, bool? ignoreCase = null)
        {
            return AddObservation(deviceKey, new ObservationInput
            {
                DeviceKey = deviceKey,
                DataItemKey = dataItemKey,
                Values = new List<ObservationValue> { new ObservationValue(valueKey, value) },
                Timestamp = UnixDateTime.Now
            });
        }

        /// <summary>
        /// Add a new Observation to the Agent for the specified Device and DataItem
        /// </summary>
        /// <param name="deviceKey">The (Name or Uuid) of the Device</param>
        /// <param name="dataItemKey">The (Name, ID, or Source) of the DataItem</param>
        /// <param name="valueKey">The ValueKey to use for the Value parameter</param>
        /// <param name="value">The Value of the Observation</param>
        /// <param name="timestamp">The Timestamp of the Observation in Unix Ticks (1/10,000 of a millisecond)</param>
        /// <param name="convertUnits">Used to override the default configuration for the Agent to ConvertUnits</param>
        /// <param name="ignoreCase">Used to override the default configuration for the Agent to IgnoreCase of the Value</param>
        /// <returns>True if the Observation was added successfully</returns>
        public bool AddObservation(string deviceKey, string dataItemKey, string valueKey, object value, long timestamp, bool? convertUnits = null, bool? ignoreCase = null)
        {
            return AddObservation(deviceKey, new ObservationInput
            {
                DeviceKey = deviceKey,
                DataItemKey = dataItemKey,
                Values = new List<ObservationValue> { new ObservationValue(valueKey, value) },
                Timestamp = timestamp
            });
        }

        /// <summary>
        /// Add a new Observation to the Agent for the specified Device and DataItem
        /// </summary>
        /// <param name="deviceKey">The (Name or Uuid) of the Device</param>
        /// <param name="dataItemKey">The (Name, ID, or Source) of the DataItem</param>
        /// <param name="valueKey">The ValueKey to use for the Value parameter</param>
        /// <param name="value">The Value of the Observation</param>
        /// <param name="timestamp">The Timestamp of the Observation</param>
        /// <param name="convertUnits">Used to override the default configuration for the Agent to ConvertUnits</param>
        /// <param name="ignoreCase">Used to override the default configuration for the Agent to IgnoreCase of the Value</param>
        /// <returns>True if the Observation was added successfully</returns>
        public bool AddObservation(string deviceKey, string dataItemKey, string valueKey, object value, DateTime timestamp, bool? convertUnits = null, bool? ignoreCase = null)
        {
            return AddObservation(deviceKey, new ObservationInput
            {
                DeviceKey = deviceKey,
                DataItemKey = dataItemKey,
                Values = new List<ObservationValue> { new ObservationValue(valueKey, value) },
                Timestamp = timestamp.ToUnixTime()
            });
        }

        /// <summary>
        /// Add a new Observation to the Agent for the specified Device and DataItem
        /// </summary>
        /// <param name="deviceKey">The (Name or Uuid) of the Device</param>
        /// <param name="observationInput">The Observation to add</param>
        /// <param name="ignoreTimestamp">Used to override the default configuration for the Agent to IgnoreTimestamp</param>
        /// <param name="convertUnits">Used to override the default configuration for the Agent to ConvertUnits</param>
        /// <param name="ignoreCase">Used to override the default configuration for the Agent to IgnoreCase of the Value</param>
        /// <returns>True if the Observation was added successfully</returns>
        public bool AddObservation(string deviceKey, IObservationInput observationInput, bool? ignoreTimestamp = null, bool? convertUnits = null, bool? ignoreCase = null)
        {
            if (observationInput != null)
            {
                ObservationReceived?.Invoke(this, observationInput);

                IObservationInput input = new ObservationInput();
                input.DeviceKey = deviceKey;
                input.DataItemKey = observationInput.DataItemKey;
                input.IsUnavailable = observationInput.IsUnavailable;

                // Convert Case (if Ignored)
                if ((!ignoreCase.HasValue && _configuration.IgnoreObservationCase) || (ignoreCase.HasValue && ignoreCase.Value))
                {
                    input.Values = Observation.UppercaseValues(observationInput.Values);
                }
                else input.Values = observationInput.Values;

                // Set Timestamp
                if ((!ignoreTimestamp.HasValue && _configuration.IgnoreTimestamps) || (ignoreTimestamp.HasValue && ignoreTimestamp.Value))
                {
                    input.Timestamp = UnixDateTime.Now;
                }
                else input.Timestamp = observationInput.Timestamp > 0 ? observationInput.Timestamp : UnixDateTime.Now;

                // Get Device UUID from deviceKey
                var deviceUuid = GetDeviceUuid(deviceKey);

                // Get DataItem based on Observation's Key
                var dataItem = GetDataItem(deviceUuid, input.DataItemKey);
                if (dataItem != null)
                {
                    // Add required properties
                    switch (dataItem.Representation)
                    {
                        case DataItemRepresentation.DATA_SET: if (input.IsUnavailable) input.AddValue(ValueKeys.Count, 0); break;
                        case DataItemRepresentation.TABLE: if (input.IsUnavailable) input.AddValue(ValueKeys.Count, 0); break;
                        case DataItemRepresentation.TIME_SERIES: if (input.IsUnavailable) input.AddValue(ValueKeys.SampleCount, 0); break;
                    }

                    // Process Observation using Processers
                    if (ProcessObservationFunction != null)
                    {
                        input = ProcessObservationFunction(new ProcessObservation(this, dataItem, input));
                    }

                    var success = false;
                    var validationResult = new ValidationResult(true);

                    if (_configuration.InputValidationLevel > InputValidationLevel.Ignore)
                    {
                        // Validate Observation Input with DataItem type
                        validationResult = dataItem.IsValid(MTConnectVersion, input);
                        if (!validationResult.IsValid) validationResult.Message = $"{dataItem.Type} : {dataItem.Id} : {validationResult.Message}";
                    }

                    if (validationResult.IsValid || _configuration.InputValidationLevel != InputValidationLevel.Strict)
                    {
                        // Convert Units (if needed)
                        if ((!convertUnits.HasValue && _configuration.ConvertUnits) || (convertUnits.HasValue && convertUnits.Value))
                        {
                            input = ConvertObservationValue(dataItem, input);
                            if (input == null) return false;
                        }

                        bool update;

                        // Check if Observation Needs to be Updated
                        if (dataItem.Category == DataItemCategory.CONDITION)
                        {
                            update = UpdateCurrentCondition(deviceUuid, dataItem, input);
                        }
                        else
                        {
                            update = UpdateCurrentObservation(deviceUuid, dataItem, input);
                        }

                        // Check if Observation Needs to be Updated
                        if (update)
                        {
                            // Call Update to Observation Buffer - HERE
                            success = OnAddObservation(deviceUuid, dataItem, input);
                            if (success)
                            {
                                if (_metrics != null)
                                {
                                    if (dataItem.Type != ObservationUpdateRateDataItem.TypeId && dataItem.Type != AssetUpdateRateDataItem.TypeId)
                                    {
                                        // Update Agent Metrics
                                        _metrics.UpdateObservation(deviceUuid, dataItem.Id);
                                    }
                                }

                                var observation = Observation.Create(dataItem);
                                observation.DeviceUuid = deviceUuid;
                                observation.DataItem = dataItem;
                                observation.InstanceId = _instanceId;
                                observation.Timestamp = input.Timestamp.ToDateTime();
                                observation.AddValues(input.Values);
                                OnObservationAdded(observation);
                            }
                        }
                        else success = true; // Return true if no update needed
                    }

                    if (!validationResult.IsValid && InvalidObservationAdded != null)
                    {
                        InvalidObservationAdded.Invoke(deviceUuid, input.DataItemKey, validationResult);
                    }

                    return success;
                }
                else if (InvalidObservationAdded != null)
                {
                    InvalidObservationAdded.Invoke(deviceUuid, input.DataItemKey, new ValidationResult(false, $"DataItemKey \"{input.DataItemKey}\" not Found in Device"));
                }
            }

            return false;
        }


        /// <summary>
        /// Add new Observations for DataItems to the Agent
        /// </summary>
        public bool AddObservations(string deviceKey, IEnumerable<IObservationInput> observationInputs)
        {
            if (!observationInputs.IsNullOrEmpty())
            {
                bool success = false;

                foreach (var observationInput in observationInputs)
                {
                    success = AddObservation(deviceKey, observationInput);
                    if (!success) break;
                }

                return success;
            }

            return false;
        }

        protected virtual bool OnAddObservation(string deviceUuid, IDataItem dataItem, IObservationInput observationInput)
        {
            return true;
        }

        public void OnObservationAdded(IObservation observation)
        {
            if (ObservationAdded != null)
            {
                ObservationAdded?.Invoke(this, observation);
            }
        }

        public void OnInvalidObservationAdded(string deviceUuid, string dataItemId, ValidationResult result)
        {
            if (InvalidObservationAdded != null)
            {
                InvalidObservationAdded?.Invoke(deviceUuid, dataItemId, result);
            }
        }

        #endregion

        #region "Assets"

        protected virtual bool OnAssetUpdate(IAsset asset)
        {
            return true;
        }

        protected virtual bool OnNewAssetAdded(IAsset asset)
        {
            return true;
        }


        /// <summary>
        /// Add a new Asset to the Agent for the specified Device and DataItem
        /// </summary>
        /// <param name="deviceKey">The (Name or Uuid) of the Device</param>
        /// <param name="asset">The Asset to add</param>
        /// <param name="ignoreTimestamp">Used to override the default configuration for the Agent to IgnoreTimestamp</param>
        /// <returns>True if the Asset was added successfully</returns>
        public bool AddAsset(string deviceKey, IAsset asset, bool? ignoreTimestamp = null)
        {
            // Get Device UUID from deviceKey
            var deviceUuid = GetDeviceUuid(deviceKey);
            if (!string.IsNullOrEmpty(deviceUuid))
            {
                // Get Device from DeviceBuffer
                _devices.TryGetValue(deviceUuid, out var device);
                if (device != null)
                {
                    // Set Device UUID Property
                    ((Asset)asset).DeviceUuid = device.Uuid;

                    // Set InstanceId
                    asset.InstanceId = _instanceId;

                    // Set Timestamp
                    if ((!ignoreTimestamp.HasValue && _configuration.IgnoreTimestamps) || (ignoreTimestamp.HasValue && ignoreTimestamp.Value))
                    {
                        ((Asset)asset).Timestamp = DateTime.Now;
                    }
                    else ((Asset)asset).Timestamp = asset.Timestamp > DateTime.Now ? asset.Timestamp : DateTime.Now;

                    // Set Asset Hash
                    ((Asset)asset).Hash = asset.GenerateHash();

                    // Validate Asset based on Device's MTConnectVersion
                    var validationResults = asset.IsValid(device.MTConnectVersion);
                    if (validationResults.IsValid || _configuration.InputValidationLevel < InputValidationLevel.Strict)
                    {
                        // Set flag whether the Asset already exists in the Buffer
                        var exists = _assetIds.Contains(asset.AssetId);

                        // Add Asset to AssetBuffer
                        if (OnAssetUpdate(asset))
                        {
                            // Update AssetChanged DataItem
                            if (!device.DataItems.IsNullOrEmpty())
                            {
                                var assetChanged = device.DataItems.FirstOrDefault(o => o.Type == AssetChangedDataItem.TypeId);
                                if (assetChanged != null)
                                {
                                    var assetChangedObservation = new ObservationInput();
                                    assetChangedObservation.DataItemKey = assetChanged.Id;
                                    assetChangedObservation.AddValue(ValueKeys.Result, asset.AssetId);
                                    assetChangedObservation.AddValue(ValueKeys.AssetType, asset.Type);
                                    assetChangedObservation.Timestamp = asset.Timestamp.ToUnixTime();
                                    AddObservation(deviceUuid, assetChangedObservation);
                                }
                            }

                            // Update Agent Metrics
                            if (_metrics != null) _metrics.UpdateAsset(deviceUuid, asset.AssetId);

                            if (!exists)
                            {
                                OnNewAssetAdded(asset);

                                // Update Asset Count
                                //IncrementAssetCount(deviceUuid, asset.Type);
                            }

                            if (!validationResults.IsValid && _configuration.InputValidationLevel > InputValidationLevel.Ignore)
                            {
                                if (InvalidAssetAdded != null) InvalidAssetAdded.Invoke(asset, validationResults);
                            }

                            AssetAdded?.Invoke(this, asset);
                            return true;
                        }
                    }
                    else
                    {
                        if (InvalidAssetAdded != null) InvalidAssetAdded.Invoke(asset, validationResults);
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Add new Assets to the Agent
        /// </summary>
        /// <param name="deviceKey">The (Name or Uuid) of the Device</param>
        /// <param name="assets">The Assets to add</param>
        /// <returns>True if the Assets was added successfully</returns>
        public bool AddAssets(string deviceKey, IEnumerable<IAsset> assets)
        {
            if (!assets.IsNullOrEmpty())
            {
                var success = false;

                foreach (var asset in assets)
                {
                    success = AddAsset(deviceKey, asset);
                    if (!success) break;
                }

                return success;
            }

            return false;
        }

        #endregion

        #endregion

        #region "Metrics"

        private void DeviceMetricsUpdated(object sender, DeviceMetrics deviceMetrics)
        {
            if (deviceMetrics != null)
            {
                _devices.TryGetValue(deviceMetrics.DeviceUuid, out var device);
                if (device != null)
                {
                    var dataItems = device.GetDataItems();
                    if (!dataItems.IsNullOrEmpty())
                    {
                        // Update ObservationUpdateRate DataItem
                        var observationUpdateRate = dataItems.FirstOrDefault(o => o.Type == ObservationUpdateRateDataItem.TypeId);
                        if (observationUpdateRate != null)
                        {
                            AddObservation(device.Name, observationUpdateRate.Id, ValueKeys.Result, deviceMetrics.ObservationAverage);
                        }

                        // Update AssetUpdateRate DataItem
                        var assetUpdateRate = dataItems.FirstOrDefault(o => o.Type == AssetUpdateRateDataItem.TypeId);
                        if (assetUpdateRate != null)
                        {
                            AddObservation(device.Name, assetUpdateRate.Id, ValueKeys.Result, deviceMetrics.AssetAverage);
                        }
                    }
                }
            }
        }

        #endregion

        #region "Agent Information"

        private void StartAgentInformationUpdateTimer()
        {
            if (_informationUpdateTimer != null) _informationUpdateTimer.Dispose();
            _informationUpdateTimer = new System.Timers.Timer();
            _informationUpdateTimer.Interval = InformationUpdateInterval;
            _informationUpdateTimer.Elapsed += UpdateAgentInformation;
            _informationUpdateTimer.Enabled = true;
        }

        private void StopAgentInformationUpdateTimer()
        {
            if (_informationUpdateTimer != null) _informationUpdateTimer.Dispose();
        }

        private void UpdateAgentInformation(object sender, EventArgs args)
        {
            if (!_updateInformation)
            {
                // Check if InstanceId is the same
                if (_information.InstanceId != _instanceId)
                {
                    _information.InstanceId = _instanceId;
                    _updateInformation = true;
                }

                // Check if DeviceModelChangeTime is the same
                if (_information.DeviceModelChangeTime != _deviceModelChangeTime)
                {
                    _information.DeviceModelChangeTime = _deviceModelChangeTime;
                    _updateInformation = true;
                }
            }

            if (_updateInformation)
            {
                // Save to File
                _information.Save();
                _updateInformation = false;
            }
        }

        #endregion


        private static long CreateInstanceId()
        {
            return UnixDateTime.Now / 1000 / 10000;
        }

        private static Version GetAgentVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version;
        }
    }
}