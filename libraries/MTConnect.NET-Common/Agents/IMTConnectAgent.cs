// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Agents.Metrics;
using MTConnect.Assets;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Devices.DataItems;
using MTConnect.Input;
using MTConnect.Observations;
using MTConnect.Observations.Output;
using System;
using System.Collections.Generic;

namespace MTConnect.Agents
{
    /// <summary>
    /// An Agent is the centerpiece of an MTConnect implementation. 
    /// It organizes and manages individual pieces of information published by one or more pieces of equipment.
    /// </summary>
    public interface IMTConnectAgent
    {
        /// <summary>
        /// Gets the Device that represents the Agent in the Information Model
        /// </summary>
        Agent Agent { get; }

        /// <summary>
        /// Gets the Configuration associated with the Agent
        /// </summary>
        IAgentConfiguration Configuration { get; }

        /// <summary>
        /// Gets the Metrics associated with the Agent
        /// </summary>
        MTConnectAgentMetrics Metrics { get; }

        /// <summary>
        /// Gets the unique identifier for the Agent
        /// </summary>
        string Uuid { get; }

        /// <summary>
        /// Gets a representation of the specific instance of the Agent.
        /// </summary>
        long InstanceId { get; }

        /// <summary>
        /// Gets the Agent Version
        /// </summary>
        Version Version { get; }

        /// <summary>
        /// Gets the MTConnect Version that the Agent is using.
        /// </summary>
        Version MTConnectVersion { get; set; }

        /// <summary>
        /// Gets the Sender that is hosting the Agent
        /// </summary>
        string Sender { get; }

        /// <summary>
        /// A timestamp in 8601 format of the last update of the Device information for any device.
        /// </summary>
        DateTime DeviceModelChangeTime { get; }



        #region "Events"

        /// <summary>
        /// Event raised when a new Device is added to the Agent
        /// </summary>
        event EventHandler<IDevice> DeviceAdded;

        /// <summary>
        /// Raised when a new Observation is attempted to be added to the Agent
        /// </summary>
        event EventHandler<IObservationInput> ObservationReceived;

        /// <summary>
        /// Raised when a new Observation is successfully added to the Agent
        /// </summary>
        event EventHandler<IObservation> ObservationAdded;

        /// <summary>
        /// Raised when a new Asset is attempted to be added to the Agent
        /// </summary>
        event EventHandler<IAsset> AssetReceived;

        /// <summary>
        /// Raised when a new Asset is added to the Agent
        /// </summary>
        event EventHandler<IAsset> AssetAdded;


        /// <summary>
        /// Raised when an Invalid Component is Added
        /// </summary>
        event MTConnectComponentValidationHandler InvalidComponentAdded;

        /// <summary>
        /// Raised when an Invalid Composition is Added
        /// </summary>
        event MTConnectCompositionValidationHandler InvalidCompositionAdded;

        /// <summary>
        /// Raised when an Invalid DataItem is Added
        /// </summary>
        event MTConnectDataItemValidationHandler InvalidDataItemAdded;

        /// <summary>
        /// Raised when an Invalid Observation is Added
        /// </summary>
        event MTConnectObservationValidationHandler InvalidObservationAdded;

        /// <summary>
        /// Raised when an Invalid Asset is Added
        /// </summary>
        event MTConnectAssetValidationHandler InvalidAssetAdded;

        #endregion

        #region "Start / Stop"

        /// <summary>
        /// Start the MTConnect Agent
        /// </summary>
        void Start();

        /// <summary>
        /// Stop the MTConnect Agent
        /// </summary>
        void Stop();

        #endregion

        #region "Entities"

        IDevice GetDevice(string deviceKey);

        IDevice GetDevice(string deviceKey, Version mtconnectVersion);

        IEnumerable<IDevice> GetDevices();

        IEnumerable<IDevice> GetDevices(Version mtconnectVersion);

        IEnumerable<IDevice> GetDevices(string deviceType);

        IEnumerable<IDevice> GetDevices(string deviceType, Version mtconnectVersion);


        IDataItem GetDataItem(string deviceKey, string dataItemKey);


        IEnumerable<IObservationOutput> GetCurrentObservations(Version mtconnectVersion = null);

        IEnumerable<IObservationOutput> GetCurrentObservations(string deviceKey, Version mtconnectVersion = null);

        IEnumerable<IObservationOutput> GetCurrentObservations(string deviceKey, string dataItemKey, Version mtconnectVersion = null);


        IEnumerable<IAsset> GetAssets(Version mtconnectVersion = null);

        IEnumerable<IAsset> GetAssets(string deviceKey, Version mtconnectVersion = null);

        /// <summary>
        /// Remove the Asset with the specified Asset ID
        /// </summary>
        /// <param name="assetId">The ID of the Asset to remove</param>
        /// <param name="timestamp">The Timestamp of when the Asset was removed in Unix Ticks (1/10,000 of a millisecond)</param>
        /// <returns>Returns True if the Asset was successfully removed</returns>
        bool RemoveAsset(string assetId, long timestamp = 0);

        /// <summary>
        /// Remove the Asset with the specified Asset ID
        /// </summary>
        /// <param name="assetId">The ID of the Asset to remove</param>
        /// <param name="timestamp">The Timestamp of when the Asset was removed</param>
        /// <returns>Returns True if the Asset was successfully removed</returns>
        bool RemoveAsset(string assetId, DateTime timestamp);


        /// <summary>
        /// Remove all Assets with the specified Type
        /// </summary>
        /// <param name="assetType">The Type of the Assets to remove</param>
        /// <param name="timestamp">The Timestamp of when the Assets were removed in Unix Ticks (1/10,000 of a millisecond)</param>
        /// <returns>Returns True if the Assets were successfully removed</returns>
        bool RemoveAllAssets(string assetType, long timestamp = 0);

        /// <summary>
        /// Remove all Assets with the specified Type
        /// </summary>
        /// <param name="assetType">The Type of the Assets to remove</param>
        /// <param name="timestamp">The Timestamp of when the Assets were removed</param>
        /// <returns>Returns True if the Assets were successfully removed</returns>
        bool RemoveAllAssets(string assetType, DateTime timestamp);

        #endregion


        #region "Add"

        /// <summary>
        /// Add a new MTConnectDevice to the Agent's Buffer
        /// </summary>
        bool AddDevice(IDevice device, bool intializeDataItems = true);

        /// <summary>
        /// Add new MTConnectDevices to the Agent's Buffer
        /// </summary>
        bool AddDevices(IEnumerable<IDevice> devices, bool intializeDataItems = true);


        /// <summary>
        /// Add a new Observation to the Agent for the specified Device and DataItem
        /// </summary>
        /// <param name="deviceKey">The (Name or Uuid) of the Device</param>
        /// <param name="dataItemKey">The (Name, ID, or Source) of the DataItem</param>
        /// <param name="value">The Value of the Observation (equivalent to ValueKey = Value)</param>
        /// <param name="convertUnits">Used to override the default configuration for the Agent to ConvertUnits</param>
        /// <param name="ignoreCase">Used to override the default configuration for the Agent to IgnoreCase of the Value</param>
        /// <returns>True if the Observation was added successfully</returns>
        bool AddObservation(string deviceKey, string dataItemKey, object value, bool? convertUnits = null, bool? ignoreCase = null);

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
        bool AddObservation(string deviceKey, string dataItemKey, object value, long timestamp, bool? convertUnits = null, bool? ignoreCase = null);

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
        bool AddObservation(string deviceKey, string dataItemKey, object value, DateTime timestamp, bool? convertUnits = null, bool? ignoreCase = null);

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
        bool AddObservation(string deviceKey, string dataItemKey, string valueKey, object value, bool? convertUnits = null, bool? ignoreCase = null);

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
        bool AddObservation(string deviceKey, string dataItemKey, string valueKey, object value, long timestamp, bool? convertUnits = null, bool? ignoreCase = null);

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
        bool AddObservation(string deviceKey, string dataItemKey, string valueKey, object value, DateTime timestamp, bool? convertUnits = null, bool? ignoreCase = null);

        /// <summary>
        /// Add a new Observation to the Agent for the specified Device and DataItem
        /// </summary>
        /// <param name="deviceKey">The (Name or Uuid) of the Device</param>
        /// <param name="observationInput">The Observation to add</param>
        /// <param name="ignoreTimestamp">Used to override the default configuration for the Agent to IgnoreTimestamp</param>
        /// <param name="convertUnits">Used to override the default configuration for the Agent to ConvertUnits</param>
        /// <param name="ignoreCase">Used to override the default configuration for the Agent to IgnoreCase of the Value</param>
        /// <returns>True if the Observation was added successfully</returns>
        bool AddObservation(string deviceKey, IObservationInput observationInput, bool? ignoreTimestamp = null, bool? convertUnits = null, bool? ignoreCase = null);

        /// <summary>
        /// Add new Observations to the Agent for the specified Device
        /// </summary>
        bool AddObservations(string deviceKey, IEnumerable<IObservationInput> observationInputs);


        /// <summary>
        /// Add a new Asset to the Agent for the specified Device and DataItem
        /// </summary>
        /// <param name="deviceKey">The (Name or Uuid) of the Device</param>
        /// <param name="asset">The Asset to add</param>
        /// <param name="ignoreTimestamp">Used to override the default configuration for the Agent to IgnoreTimestamp</param>
        /// <returns>True if the Asset was added successfully</returns>
        bool AddAsset(string deviceKey, IAsset asset, bool? ignoreTimestamp = null);

        /// <summary>
        /// Add new Assets to the Agent
        /// </summary>
        /// <param name="deviceKey">The (Name or Uuid) of the Device</param>
        /// <param name="assets">The Assets to add</param>
        /// <returns>True if the Assets was added successfully</returns>
        bool AddAssets(string deviceKey, IEnumerable<IAsset> assets);


        void OnObservationAdded(IObservation observation);

        void OnInvalidObservationAdded(string deviceUuid, string dataItemId, ValidationResult result);

        #endregion

        #region "Interfaces"

        // Task<IEnumerable<Interfaces.Interface>> GetInterfaces();

        // Task<IEnumerable<Interfaces.Interface>> GetInterfaces(string deviceName);

        // Task<IEnumerable<Interfaces.Interface>> GetInterfaces(IEnumerable<string> deviceNames);

        // Task<Interfaces.Interface> GetInterfaces(string deviceName, string interfaceId);

        // Task<IEnumerable<Interfaces.Interface>> GetInterfaces(string deviceName, IEnumerable<string> interfaceIds);


        // Task<Interfaces.InterfaceState> GetInterfaceState(string deviceName, string interfaceId);

        // Task<Interfaces.InterfaceRequestState> GetRequestState(string deviceName, string interfaceId);

        // Task<Interfaces.InterfaceResponseState> GetResponseState(string deviceName, string interfaceId);

        #endregion

    }
}