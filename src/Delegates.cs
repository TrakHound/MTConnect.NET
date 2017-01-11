// Copyright (c) 2017 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System;

namespace MTConnect
{
    public delegate void MTConnectDevicesHandler(MTConnectDevices.Document document);

    public delegate void MTConnectStreamsHandler(MTConnectStreams.Document document);

    public delegate void MTConnectAssetsHandler(MTConnectAssets.Document document);

    public delegate void MTConnectErrorHandler(MTConnectError.Document errorDocument);

    public delegate void XmlHandler(string xml);

    public delegate void ConnectionErrorHandler(Exception ex);

    public delegate void StreamStatusHandler();
}
