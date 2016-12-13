// Copyright (c) 2016 Feenux LLC, All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace MTConnect.Client
{
    public delegate void MTConnectDevicesHandler(v13.MTConnectDevices.Document document);

    public delegate void MTConnectStreamsHandler(v13.MTConnectStreams.Document document);

    public delegate void MTConnectAssetsHandler(v13.MTConnectAssets.Document document);

    public delegate void MTConnectErrorHandler(v13.MTConnectError.Document errorDocument);

    public delegate void StreamStatusHandler();

    public delegate void DocumentHandler(string xml);

    public delegate void ConnectionErrorHandler(Exception ex);
}
