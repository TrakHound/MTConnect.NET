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
    public delegate void MTConnectDevicesHandler(IDevicesResponseDocument document);
    public delegate void MTConnectDevicesRequestedHandler(string deviceUuid);

    public delegate void MTConnectStreamsHandler(string documentId);
    public delegate void MTConnectStreamsRequestedHandler(string deviceUuid);

    public delegate void MTConnectAssetsHandler(IAssetsResponseDocument document);
    public delegate void MTConnectAssetsRequestedHandler(IEnumerable<string> assetIds);
    public delegate void MTConnectDeviceAssetsRequestedHandler(string deviceUuid);

    public delegate void MTConnectErrorHandler(IErrorResponseDocument errorDocument);

    public delegate void MTConnectComponentValidationHandler(string deviceUuid, IComponent component, ValidationResult validationResults);
    public delegate void MTConnectCompositionValidationHandler(string deviceUuid, IComposition composition, ValidationResult validationResults);
    public delegate void MTConnectDataItemValidationHandler(string deviceUuid, IDataItem dataItem, ValidationResult validationResults);
    public delegate void MTConnectObservationValidationHandler(string deviceUuid, string dataItemKey, ValidationResult validationResults);
    public delegate void MTConnectAssetValidationHandler(IAsset asset, AssetValidationResult validationResults);

    public delegate void XmlHandler(string xml);

    public delegate void ConnectionErrorHandler(Exception ex);

    public delegate void InternalErrorHandler(Exception ex);

    public delegate void ClientStatusHandler(string url);

    public delegate void StreamStatusHandler(string url);


    public delegate void AdapterDataItemHandler(string deviceKey, string dataItemKey, string valueKey, string value, long timestamp);

    public delegate void AdapterAssetHandler(string deviceKey, IAsset asset);
}