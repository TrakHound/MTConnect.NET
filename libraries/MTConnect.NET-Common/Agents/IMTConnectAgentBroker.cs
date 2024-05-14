// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using MTConnect.Devices;
using MTConnect.Errors;
using MTConnect.Streams.Output;
using System;
using System.Collections.Generic;

namespace MTConnect.Agents
{
    /// <summary>
    /// Retrieve MTConnect information in the form of MTConnect Response Documents.
    /// </summary>
    public interface IMTConnectAgentBroker : IMTConnectAgent
    {
        /// <summary>
        /// Get the configured size of the Buffer in the number of maximum number of DataItems the buffer can hold at one time.
        /// </summary>
        ulong BufferSize { get; }

        /// <summary>
        /// Get the configured size of the Asset Buffer in the number of maximum number of Assets the buffer can hold at one time.
        /// </summary>
        ulong AssetBufferSize { get; }

        /// <summary>
        /// A number representing the current number of Asset Documents that are currently stored in the Agent.
        /// </summary>
        ulong AssetCount { get; }

        /// <summary>
        /// A number representing the sequence number assigned to the oldest piece of Streaming Data stored in the buffer
        /// </summary>
        ulong FirstSequence { get; }

        /// <summary>
        /// A number representing the sequence number assigned to the last piece of Streaming Data that was added to the buffer
        /// </summary>
        ulong LastSequence { get; }

        /// <summary>
        /// A number representing the sequence number of the piece of Streaming Data that is the next piece of data to be retrieved from the buffer
        /// </summary>
        ulong NextSequence { get; }


        Dictionary<string, int> DeviceIndexes { get; }

        Dictionary<string, int> DataItemIndexes { get; }


        #region "Events"

        /// <summary>
        /// Raised when an MTConnectDevices response Document is requested from the Agent
        /// </summary>
        event MTConnectDevicesRequestedHandler DevicesRequestReceived;

        /// <summary>
        /// Raised when an MTConnectDevices response Document is sent successfully from the Agent
        /// </summary>
        event MTConnectDevicesHandler DevicesResponseSent;

        /// <summary>
        /// Raised when an MTConnectStreams response Document is requested from the Agent
        /// </summary>
        event MTConnectStreamsRequestedHandler StreamsRequestReceived;

        /// <summary>
        /// Raised when an MTConnectStreams response Document is sent successfully from the Agent
        /// </summary>
        event EventHandler StreamsResponseSent;

        /// <summary>
        /// Raised when an MTConnectAssets response Document is requested from the Agent
        /// </summary>
        event MTConnectAssetsRequestedHandler AssetsRequestReceived;

        /// <summary>
        /// Raised when an MTConnectAssets response Document is requested from the Agent for a specific Device
        /// </summary>
        event MTConnectDeviceAssetsRequestedHandler DeviceAssetsRequestReceived;

        /// <summary>
        /// Raised when an MTConnectAssets response Document is sent successfully from the Agent
        /// </summary>
        event MTConnectAssetsHandler AssetsResponseSent;

        /// <summary>
        /// Raised when an MTConnectError response Document is sent successfully from the Agent
        /// </summary>
        event MTConnectErrorHandler ErrorResponseSent;

        #endregion

        #region "DevicesResponseDocuments"

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

        #region "StreamsResponseDocuments"

        /// <summary>
        /// Get a MTConnectStreams Document containing all devices.
        /// </summary>
        /// <param name="count">The Maximum Number of DataItems to return</param>
        /// <returns>MTConnectStreams Response Document</returns>
        IStreamsResponseOutputDocument GetDeviceStreamsResponseDocument(uint count = 0, Version mtconnectVersion = null, string deviceType = null);

        /// <summary>
        /// Get a MTConnectStreams Document containing all devices.
        /// </summary>
        /// <param name="at">The sequence number to include in the response</param>
        /// <param name="count">The maximum number of observations to include in the response</param>
        /// <returns>MTConnectStreams Response Document</returns>
        IStreamsResponseOutputDocument GetDeviceStreamsResponseDocument(ulong at, uint count = 0, Version mtconnectVersion = null, string deviceType = null);

        /// <summary>
        /// Get a MTConnectStreams Document containing all devices.
        /// </summary>
        /// <param name="dataItemIds">A list of DataItemId's to specify what observations to include in the response</param>
        /// <param name="count">The maximum number of observations to include in the response</param>
        /// <returns>MTConnectStreams Response Document</returns>
        IStreamsResponseOutputDocument GetDeviceStreamsResponseDocument(IEnumerable<string> dataItemIds, uint count = 0, Version mtconnectVersion = null, string deviceType = null);


        /// <summary>
        /// Get a MTConnectStreams Document containing all devices.
        /// </summary>
        /// <param name="dataItemIds">A list of DataItemId's to specify what observations to include in the response</param>
        /// <param name="at">The sequence number to include in the response</param>
        /// <param name="count">The maximum number of observations to include in the response</param>
        /// <returns>MTConnectStreams Response Document</returns>
        IStreamsResponseOutputDocument GetDeviceStreamsResponseDocument(IEnumerable<string> dataItemIds, ulong at, uint count = 0, Version mtconnectVersion = null, string deviceType = null);

        /// <summary>
        /// Get a MTConnectStreams Document containing all devices.
        /// </summary>
        /// <param name="from">The sequence number of the first observation to include in the response</param>
        /// <param name="to">The sequence number of the last observation to include in the response</param>
        /// <param name="count">The maximum number of observations to include in the response</param>
        /// <returns>MTConnectStreams Response Document</returns>
        IStreamsResponseOutputDocument GetDeviceStreamsResponseDocument(ulong from, ulong to, uint count = 0, Version mtconnectVersion = null, string deviceType = null);

        /// <summary>
        /// Get a MTConnectStreams Document containing all devices.
        /// </summary>
        /// <param name="dataItemIds">A list of DataItemId's to specify what observations to include in the response</param>
        /// <param name="from">The sequence number of the first observation to include in the response</param>
        /// <param name="to">The sequence number of the last observation to include in the response</param>
        /// <param name="count">The maximum number of observations to include in the response</param>
        /// <returns>MTConnectStreams Response Document</returns>
        IStreamsResponseOutputDocument GetDeviceStreamsResponseDocument(IEnumerable<string> dataItemIds, ulong from, ulong to, uint count = 0, Version mtconnectVersion = null, string deviceType = null);


        /// <summary>
        /// Get a MTConnectStreams Document containing the specified Device.
        /// </summary>
        /// <param name="deviceKey">The (name or uuid) of the requested Device</param>
        /// <param name="count">The maximum number of observations to include in the response</param>
        /// <returns>MTConnectStreams Response Document</returns>
        IStreamsResponseOutputDocument GetDeviceStreamsResponseDocument(string deviceKey, uint count = 0, Version mtconnectVersion = null);

        /// <summary>
        /// Get a MTConnectStreams Document containing the specified Device.
        /// </summary>
        /// <param name="deviceKey">The (name or uuid) of the requested Device</param>
        /// <param name="dataItemIds">A list of DataItemId's to specify what observations to include in the response</param>
        /// <param name="count">The maximum number of observations to include in the response</param>
        /// <returns>MTConnectStreams Response Document</returns>
        IStreamsResponseOutputDocument GetDeviceStreamsResponseDocument(string deviceKey, IEnumerable<string> dataItemIds, uint count = 0, Version mtconnectVersion = null);

        /// <summary>
        /// Get a MTConnectStreams Document containing the specified Device.
        /// </summary>
        /// <param name="deviceKey">The (name or uuid) of the requested Device</param>
        /// <param name="at">The sequence number to include in the response</param>
        /// <param name="count">The maximum number of observations to include in the response</param>
        /// <returns>MTConnectStreams Response Document</returns>
        IStreamsResponseOutputDocument GetDeviceStreamsResponseDocument(string deviceKey, ulong at, uint count = 0, Version mtconnectVersion = null);

        /// <summary>
        /// Get a MTConnectStreams Document containing the specified Device.
        /// </summary>
        /// <param name="deviceKey">The (name or uuid) of the requested Device</param>
        /// <param name="dataItemIds">A list of DataItemId's to specify what observations to include in the response</param>
        /// <param name="at">The sequence number to include in the response</param>
        /// <param name="count">The maximum number of observations to include in the response</param>
        /// <returns>MTConnectStreams Response Document</returns>
        IStreamsResponseOutputDocument GetDeviceStreamsResponseDocument(string deviceKey, IEnumerable<string> dataItemIds, ulong at, uint count = 0, Version mtconnectVersion = null);

        /// <summary>
        /// Get a MTConnectStreams Document containing the specified Device.
        /// </summary>
        /// <param name="deviceKey">The (name or uuid) of the requested Device</param>
        /// <param name="from">The sequence number of the first observation to include in the response</param>
        /// <param name="to">The sequence number of the last observation to include in the response</param>
        /// <param name="count">The maximum number of observations to include in the response</param>
        /// <returns>MTConnectStreams Response Document</returns>
        IStreamsResponseOutputDocument GetDeviceStreamsResponseDocument(string deviceKey, ulong from, ulong to, uint count = 0, Version mtconnectVersion = null);

        /// <summary>
        /// Get a MTConnectStreams Document containing the specified Device.
        /// </summary>
        /// <param name="deviceKey">The (name or uuid) of the requested Device</param>
        /// <param name="dataItemIds">A list of DataItemId's to specify what observations to include in the response</param>
        /// <param name="from">The sequence number of the first observation to include in the response</param>
        /// <param name="to">The sequence number of the last observation to include in the response</param>
        /// <param name="count">The maximum number of observations to include in the response</param>
        /// <returns>MTConnectStreams Response Document</returns>
        IStreamsResponseOutputDocument GetDeviceStreamsResponseDocument(string deviceKey, IEnumerable<string> dataItemIds, ulong from, ulong to, uint count = 0, Version mtconnectVersion = null);

        #endregion

        #region "AssetsResponseDocuments"

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
        IAssetsResponseDocument GetAssetsResponseDocument(string deviceKey = null, string type = null, bool removed = false, uint count = 100, Version mtconnectVersion = null);

        /// <summary>
        /// Get a MTConnectAssets Document containing the specified Asset
        /// </summary>
        /// <param name="assetIds">The IDs of the Assets to include in the response</param>
        /// <returns>MTConnectAssets Response Document</returns>
        IAssetsResponseDocument GetAssetsResponseDocument(IEnumerable<string> assetIds, Version mtconnectVersion = null);


        ///// <summary>
        ///// Remove the Asset with the specified Asset ID
        ///// </summary>
        ///// <param name="assetId">The ID of the Asset to remove</param>
        ///// <param name="timestamp">The Timestamp of when the Asset was removed in Unix Ticks (1/10,000 of a millisecond)</param>
        ///// <returns>Returns True if the Asset was successfully removed</returns>
        //bool RemoveAsset(string assetId, long timestamp = 0);

        ///// <summary>
        ///// Remove the Asset with the specified Asset ID
        ///// </summary>
        ///// <param name="assetId">The ID of the Asset to remove</param>
        ///// <param name="timestamp">The Timestamp of when the Asset was removed</param>
        ///// <returns>Returns True if the Asset was successfully removed</returns>
        //bool RemoveAsset(string assetId, DateTime timestamp);


        ///// <summary>
        ///// Remove all Assets with the specified Type
        ///// </summary>
        ///// <param name="assetType">The Type of the Assets to remove</param>
        ///// <param name="timestamp">The Timestamp of when the Assets were removed in Unix Ticks (1/10,000 of a millisecond)</param>
        ///// <returns>Returns True if the Assets were successfully removed</returns>
        //bool RemoveAllAssets(string assetType, long timestamp = 0);

        ///// <summary>
        ///// Remove all Assets with the specified Type
        ///// </summary>
        ///// <param name="assetType">The Type of the Assets to remove</param>
        ///// <param name="timestamp">The Timestamp of when the Assets were removed</param>
        ///// <returns>Returns True if the Assets were successfully removed</returns>
        //bool RemoveAllAssets(string assetType, DateTime timestamp);

        #endregion

        #region "ErrorsResponseDocuments"

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

    }
}