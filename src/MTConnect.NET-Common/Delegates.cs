// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Assets;
using MTConnect.Devices;
using MTConnect.Errors;
using MTConnect.Streams;
using System;

namespace MTConnect
{
    public delegate void MTConnectDevicesHandler(IDevicesResponseDocument document);
    public delegate void MTConnectDevicesRequestedHandler(string deviceUuid);

    public delegate void MTConnectStreamsHandler(IStreamsResponseDocument document);
    public delegate void MTConnectStreamsRequestedHandler(string deviceUuid);

    public delegate void MTConnectAssetsHandler(IAssetsResponseDocument document);
    public delegate void MTConnectAssetsRequestedHandler(string deviceUuid);

    public delegate void MTConnectErrorHandler(IErrorResponseDocument errorDocument);

    public delegate void MTConnectDataItemValidationHandler(IDataItem dataItem, DataItemValidationResult validationResults);
    public delegate void MTConnectAssetValidationHandler(IAsset asset, AssetValidationResult validationResults);

    public delegate void XmlHandler(string xml);

    public delegate void ConnectionErrorHandler(Exception ex);

    public delegate void InternalErrorHandler(Exception ex);

    public delegate void ClientStatusHandler(string url);

    public delegate void StreamStatusHandler(string url);


    public delegate void AdapterDataItemHandler(string deviceKey, string dataItemKey, string valueKey, string value, long timestamp);

    public delegate void AdapterAssetHandler(string deviceKey, IAsset asset);
}
