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
    public delegate void MTConnectDevicesHandler(DevicesDocument document);
    public delegate void MTConnectDevicesRequestedHandler(string deviceName);

    public delegate void MTConnectStreamsHandler(StreamsDocument document);
    public delegate void MTConnectStreamsRequestedHandler(string deviceName);

    public delegate void MTConnectAssetsHandler(AssetsDocument document);
    public delegate void MTConnectAssetsRequestedHandler(string deviceName);

    public delegate void MTConnectErrorHandler(ErrorDocument errorDocument);

    public delegate void MTConnectDataItemValidationHandler(Devices.DataItem dataItem, DataItemValidationResult validationResults);
    public delegate void MTConnectAssetValidationHandler(IAsset asset, AssetValidationResult validationResults);

    public delegate void XmlHandler(string xml);

    public delegate void ConnectionErrorHandler(Exception ex);

    public delegate void InternalErrorHandler(Exception ex);

    public delegate void ClientStatusHandler(string url);

    public delegate void StreamStatusHandler(string url);


    public delegate void AdapterDataItemHandler(string deviceName, string dataItemId, string valueType, string value, long timestamp);

    public delegate void AdapterAssetHandler(string deviceName, IAsset asset);
}
