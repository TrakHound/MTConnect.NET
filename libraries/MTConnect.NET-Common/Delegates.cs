// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using MTConnect.Devices;
using MTConnect.Devices.DataItems;
using MTConnect.Errors;
using System;
using System.Collections.Generic;

namespace MTConnect
{
    /// <summary>Handles a received MTConnectDevices response document.</summary>
    public delegate void MTConnectDevicesHandler(IDevicesResponseDocument document);
    /// <summary>Handles a request for the Devices of the given device UUID.</summary>
    public delegate void MTConnectDevicesRequestedHandler(string deviceUuid);

    /// <summary>Handles a received MTConnectStreams document, identified by its document ID.</summary>
    public delegate void MTConnectStreamsHandler(string documentId);
    /// <summary>Handles a request for the Streams of the given device UUID.</summary>
    public delegate void MTConnectStreamsRequestedHandler(string deviceUuid);

    /// <summary>Handles a received MTConnectAssets response document.</summary>
    public delegate void MTConnectAssetsHandler(IAssetsResponseDocument document);
    /// <summary>Handles a request for the assets with the given asset IDs.</summary>
    public delegate void MTConnectAssetsRequestedHandler(IEnumerable<string> assetIds);
    /// <summary>Handles a request for all assets belonging to the given device UUID.</summary>
    public delegate void MTConnectDeviceAssetsRequestedHandler(string deviceUuid);

    /// <summary>Handles a received MTConnectError response document.</summary>
    public delegate void MTConnectErrorHandler(IErrorResponseDocument errorDocument);

    /// <summary>Handles the validation result for a Device, raised when the agent rejects a Device (for example, when its UUID is missing).</summary>
    public delegate void MTConnectDeviceValidationHandler(IDevice device, ValidationResult validationResults);
    /// <summary>Handles the validation result for a Component of the given device.</summary>
    public delegate void MTConnectComponentValidationHandler(string deviceUuid, IComponent component, ValidationResult validationResults);
    /// <summary>Handles the validation result for a Composition of the given device.</summary>
    public delegate void MTConnectCompositionValidationHandler(string deviceUuid, IComposition composition, ValidationResult validationResults);
    /// <summary>Handles the validation result for a DataItem of the given device.</summary>
    public delegate void MTConnectDataItemValidationHandler(string deviceUuid, IDataItem dataItem, ValidationResult validationResults);
    /// <summary>Handles the validation result for an observation of the given device DataItem.</summary>
    public delegate void MTConnectObservationValidationHandler(string deviceUuid, string dataItemKey, ValidationResult validationResults);
    /// <summary>Handles the validation result for an Asset.</summary>
    public delegate void MTConnectAssetValidationHandler(IAsset asset, ValidationResult validationResults);

    /// <summary>Handles a raw XML payload.</summary>
    public delegate void XmlHandler(string xml);

    /// <summary>Handles a connection-level error.</summary>
    public delegate void ConnectionErrorHandler(Exception ex);

    /// <summary>Handles an internal (non-connection) error.</summary>
    public delegate void InternalErrorHandler(Exception ex);

    /// <summary>Handles a client status change for the given endpoint URL.</summary>
    public delegate void ClientStatusHandler(string url);

    /// <summary>Handles a stream status change for the given endpoint URL.</summary>
    public delegate void StreamStatusHandler(string url);


    /// <summary>Handles a DataItem value received from an adapter, including its key components and timestamp.</summary>
    public delegate void AdapterDataItemHandler(string deviceKey, string dataItemKey, string valueKey, string value, long timestamp);

    /// <summary>Handles an Asset received from an adapter for the given device.</summary>
    public delegate void AdapterAssetHandler(string deviceKey, IAsset asset);
}