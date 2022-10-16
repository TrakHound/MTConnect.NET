// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Agents.Metrics;
using MTConnect.Assets;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Errors;
using MTConnect.Observations;
using MTConnect.Observations.Input;
using MTConnect.Streams.Output;
using System;
using System.Collections.Generic;

namespace MTConnect.Agents
{
    /// <summary>
    /// An Agent is the centerpiece of an MTConnect implementation. 
    /// It provides two primary functions:
    /// Organizes and manages individual pieces of information published by one or more pieces of equipment.
    /// Publishes that information in the form of a Response Document to client software applications.
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
        /// Get the configured size of the Buffer in the number of maximum number of DataItems the buffer can hold at one time.
        /// </summary>
        long BufferSize { get; }

        /// <summary>
        /// Get the configured size of the Asset Buffer in the number of maximum number of Assets the buffer can hold at one time.
        /// </summary>
        long AssetBufferSize { get; }

        /// <summary>
        /// A number representing the sequence number assigned to the oldest piece of Streaming Data stored in the buffer
        /// </summary>
        long FirstSequence { get; }

        /// <summary>
        /// A number representing the sequence number assigned to the last piece of Streaming Data that was added to the buffer
        /// </summary>
        long LastSequence { get; }

        /// <summary>
        /// A number representing the sequence number of the piece of Streaming Data that is the next piece of data to be retrieved from the buffer
        /// </summary>
        long NextSequence { get; }


        Dictionary<string, int> DeviceIndexes { get; }

        Dictionary<string, int> DataItemIndexes { get; }


        #region "Event Handlers"

        /// <summary>
        /// Event raised when a new Device is added to the Agent
        /// </summary>
        EventHandler<IDevice> DeviceAdded { get; set; }

        /// <summary>
        /// Raised when a new Observation is attempted to be added to the Agent
        /// </summary>
        EventHandler<IObservationInput> ObservationReceived { get; set; }

        /// <summary>
        /// Raised when a new Observation is successfully added to the Agent
        /// </summary>
        EventHandler<IObservation> ObservationAdded { get; set; }

        /// <summary>
        /// Raised when a new Asset is attempted to be added to the Agent
        /// </summary>
        EventHandler<IAsset> AssetReceived { get; set; }

        /// <summary>
        /// Raised when a new Asset is added to the Agent
        /// </summary>
        EventHandler<IAsset> AssetAdded { get; set; }


        /// <summary>
        /// Raised when an MTConnectDevices response Document is requested from the Agent
        /// </summary>
        MTConnectDevicesRequestedHandler DevicesRequestReceived { get; set; }

        /// <summary>
        /// Raised when an MTConnectDevices response Document is sent successfully from the Agent
        /// </summary>
        MTConnectDevicesHandler DevicesResponseSent { get; set; }

        /// <summary>
        /// Raised when an MTConnectStreams response Document is requested from the Agent
        /// </summary>
        MTConnectStreamsRequestedHandler StreamsRequestReceived { get; set; }

        /// <summary>
        /// Raised when an MTConnectStreams response Document is sent successfully from the Agent
        /// </summary>
        EventHandler StreamsResponseSent { get; set; }

        /// <summary>
        /// Raised when an MTConnectAssets response Document is requested from the Agent
        /// </summary>
        MTConnectAssetsRequestedHandler AssetsRequestReceived { get; set; }

        /// <summary>
        /// Raised when an MTConnectAssets response Document is requested from the Agent for a specific Device
        /// </summary>
        MTConnectDeviceAssetsRequestedHandler DeviceAssetsRequestReceived { get; set; }

        /// <summary>
        /// Raised when an MTConnectAssets response Document is sent successfully from the Agent
        /// </summary>
        MTConnectAssetsHandler AssetsResponseSent { get; set; }

        /// <summary>
        /// Raised when an MTConnectError response Document is sent successfully from the Agent
        /// </summary>
        MTConnectErrorHandler ErrorResponseSent { get; set; }


        /// <summary>
        /// Raised when an Invalid Component is Added
        /// </summary>
        MTConnectComponentValidationHandler InvalidComponentAdded { get; set; }

        /// <summary>
        /// Raised when an Invalid Composition is Added
        /// </summary>
        MTConnectCompositionValidationHandler InvalidCompositionAdded { get; set; }

        /// <summary>
        /// Raised when an Invalid DataItem is Added
        /// </summary>
        MTConnectDataItemValidationHandler InvalidDataItemAdded { get; set; }

        /// <summary>
        /// Raised when an Invalid Observation is Added
        /// </summary>
        MTConnectObservationValidationHandler InvalidObservationAdded { get; set; }

        /// <summary>
        /// Raised when an Invalid Asset is Added
        /// </summary>
        MTConnectAssetValidationHandler InvalidAssetAdded { get; set; }

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

        #region "Devices"

        /// <summary>
        /// Get a MTConnectDevices Response Document containing all devices.
        /// </summary>
        /// <returns>MTConnectDevices Response Document</returns>
        IDevicesResponseDocument GetDevicesResponseDocument(Version mtconnectVersion = null, string deviceType = null);

        /// <summary>
        /// Get a MTConnectDevices Response Document containing the specified device.
        /// </summary>
        /// <param name="deviceKey">The (name or uuid) of the requested Device</param>
        /// <returns>MTConnectDevices Response Document</returns>
        IDevicesResponseDocument GetDevicesResponseDocument(string deviceKey, Version mtconnectVersion = null);

        #endregion

        #region "Streams"

        /// <summary>
        /// Get a MTConnectStreams Document containing all devices.
        /// </summary>
        /// <param name="count">The Maximum Number of DataItems to return</param>
        /// <returns>MTConnectStreams Response Document</returns>
        IStreamsResponseOutputDocument GetDeviceStreamsResponseDocument(int count = 0, Version mtconnectVersion = null, string deviceType = null);

        /// <summary>
        /// Get a MTConnectStreams Document containing all devices.
        /// </summary>
        /// <param name="at">The sequence number to include in the response</param>
        /// <param name="count">The maximum number of observations to include in the response</param>
        /// <returns>MTConnectStreams Response Document</returns>
        IStreamsResponseOutputDocument GetDeviceStreamsResponseDocument(long at, int count = 0, Version mtconnectVersion = null, string deviceType = null);

        /// <summary>
        /// Get a MTConnectStreams Document containing all devices.
        /// </summary>
        /// <param name="dataItemIds">A list of DataItemId's to specify what observations to include in the response</param>
        /// <param name="at">The sequence number to include in the response</param>
        /// <param name="count">The maximum number of observations to include in the response</param>
        /// <returns>MTConnectStreams Response Document</returns>
        IStreamsResponseOutputDocument GetDeviceStreamsResponseDocument(IEnumerable<string> dataItemIds, long at, int count = 0, Version mtconnectVersion = null, string deviceType = null);

        /// <summary>
        /// Get a MTConnectStreams Document containing all devices.
        /// </summary>
        /// <param name="from">The sequence number of the first observation to include in the response</param>
        /// <param name="to">The sequence number of the last observation to include in the response</param>
        /// <param name="count">The maximum number of observations to include in the response</param>
        /// <returns>MTConnectStreams Response Document</returns>
        IStreamsResponseOutputDocument GetDeviceStreamsResponseDocument(long from, long to, int count = 0, Version mtconnectVersion = null, string deviceType = null);

        /// <summary>
        /// Get a MTConnectStreams Document containing all devices.
        /// </summary>
        /// <param name="dataItemIds">A list of DataItemId's to specify what observations to include in the response</param>
        /// <param name="from">The sequence number of the first observation to include in the response</param>
        /// <param name="to">The sequence number of the last observation to include in the response</param>
        /// <param name="count">The maximum number of observations to include in the response</param>
        /// <returns>MTConnectStreams Response Document</returns>
        IStreamsResponseOutputDocument GetDeviceStreamsResponseDocument(IEnumerable<string> dataItemIds, long from, long to, int count = 0, Version mtconnectVersion = null, string deviceType = null);


        /// <summary>
        /// Get a MTConnectStreams Document containing the specified Device.
        /// </summary>
        /// <param name="deviceKey">The (name or uuid) of the requested Device</param>
        /// <param name="count">The maximum number of observations to include in the response</param>
        /// <returns>MTConnectStreams Response Document</returns>
        IStreamsResponseOutputDocument GetDeviceStreamsResponseDocument(string deviceKey, int count = 0, Version mtconnectVersion = null);

        /// <summary>
        /// Get a MTConnectStreams Document containing the specified Device.
        /// </summary>
        /// <param name="deviceKey">The (name or uuid) of the requested Device</param>
        /// <param name="dataItemIds">A list of DataItemId's to specify what observations to include in the response</param>
        /// <param name="count">The maximum number of observations to include in the response</param>
        /// <returns>MTConnectStreams Response Document</returns>
        IStreamsResponseOutputDocument GetDeviceStreamsResponseDocument(string deviceKey, IEnumerable<string> dataItemIds, int count = 0, Version mtconnectVersion = null);

        /// <summary>
        /// Get a MTConnectStreams Document containing the specified Device.
        /// </summary>
        /// <param name="deviceKey">The (name or uuid) of the requested Device</param>
        /// <param name="at">The sequence number to include in the response</param>
        /// <param name="count">The maximum number of observations to include in the response</param>
        /// <returns>MTConnectStreams Response Document</returns>
        IStreamsResponseOutputDocument GetDeviceStreamsResponseDocument(string deviceKey, long at, int count = 0, Version mtconnectVersion = null);

        /// <summary>
        /// Get a MTConnectStreams Document containing the specified Device.
        /// </summary>
        /// <param name="deviceKey">The (name or uuid) of the requested Device</param>
        /// <param name="dataItemIds">A list of DataItemId's to specify what observations to include in the response</param>
        /// <param name="at">The sequence number to include in the response</param>
        /// <param name="count">The maximum number of observations to include in the response</param>
        /// <returns>MTConnectStreams Response Document</returns>
        IStreamsResponseOutputDocument GetDeviceStreamsResponseDocument(string deviceKey, IEnumerable<string> dataItemIds, long at, int count = 0, Version mtconnectVersion = null);

        /// <summary>
        /// Get a MTConnectStreams Document containing the specified Device.
        /// </summary>
        /// <param name="deviceKey">The (name or uuid) of the requested Device</param>
        /// <param name="from">The sequence number of the first observation to include in the response</param>
        /// <param name="to">The sequence number of the last observation to include in the response</param>
        /// <param name="count">The maximum number of observations to include in the response</param>
        /// <returns>MTConnectStreams Response Document</returns>
        IStreamsResponseOutputDocument GetDeviceStreamsResponseDocument(string deviceKey, long from, long to, int count = 0, Version mtconnectVersion = null);

        /// <summary>
        /// Get a MTConnectStreams Document containing the specified Device.
        /// </summary>
        /// <param name="deviceKey">The (name or uuid) of the requested Device</param>
        /// <param name="dataItemIds">A list of DataItemId's to specify what observations to include in the response</param>
        /// <param name="from">The sequence number of the first observation to include in the response</param>
        /// <param name="to">The sequence number of the last observation to include in the response</param>
        /// <param name="count">The maximum number of observations to include in the response</param>
        /// <returns>MTConnectStreams Response Document</returns>
        IStreamsResponseOutputDocument GetDeviceStreamsResponseDocument(string deviceKey, IEnumerable<string> dataItemIds, long from, long to, int count = 0, Version mtconnectVersion = null);

        #endregion

        #region "Assets"

        /// <summary>
        /// Get an MTConnectAssets Document containing all Assets.
        /// </summary>
        /// <param name="deviceKey">Optional  Device name or uuid. If not given, all devices are returned.</param>
        /// <param name="type">Defines the type of MTConnect Asset to be returned in the MTConnectAssets Response Document.</param>
        /// <param name="removed">
        /// An attribute that indicates whether the Asset has been removed from a piece of equipment.
        /// If the value of the removed parameter in the query is true, then Asset Documents for Assets that have been marked as removed from a piece of equipment will be included in the Response Document.
        /// If the value of the removed parameter in the query is false, then Asset Documents for Assets that have been marked as removed from a piece of equipment will not be included in the Response Document.
        /// </param>
        /// <param name="count">Defines the maximum number of Asset Documents to return in an MTConnectAssets Response Document.</param>
        /// <returns>MTConnectAssets Response Document</returns>
        IAssetsResponseDocument GetAssetsResponseDocument(string deviceKey = null, string type = null, bool removed = false, int count = 100, Version mtconnectVersion = null);

        /// <summary>
        /// Get a MTConnectAssets Document containing the specified Asset
        /// </summary>
        /// <param name="assetIds">The IDs of the Assets to include in the response</param>
        /// <returns>MTConnectAssets Response Document</returns>
        IAssetsResponseDocument GetAssetsResponseDocument(IEnumerable<string> assetIds, Version mtconnectVersion = null);


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

        #region "Errors"

        /// <summary>
        /// Get an MTConnectErrors Document containing the specified ErrorCode
        /// </summary>
        /// <param name="errorCode">Provides a descriptive code that indicates the type of error that was encountered by an Agent when attempting to respond to a Request for information.</param>
        /// <param name="value">A textual description of the error and any additional information an Agent is capable of providing regarding a specific error.</param>
        /// <returns>MTConnectError Response Document</returns>
        IErrorResponseDocument GetErrorResponseDocument(ErrorCode errorCode, string value = null, Version mtconnectVersion = null);

        /// <summary>
        /// Get an MTConnectErrors Document containing the specified Errors
        /// </summary>
        /// <param name="errors">A list of Errors to include in the response Document</param>
        /// <returns>MTConnectError Response Document</returns>
        IErrorResponseDocument GetErrorResponseDocument(IEnumerable<IError> errors, Version mtconnectVersion = null);

        #endregion

        #region "DataItems"

        IDataItem GetDataItem(string deviceKey, string dataItemKey);

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
