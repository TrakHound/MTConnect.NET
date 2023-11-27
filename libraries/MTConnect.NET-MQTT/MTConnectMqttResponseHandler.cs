// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;

namespace MTConnect
{
    public delegate void MTConnectMqttResponseHandler<TResponseDocument>(IDevice device, TResponseDocument responseDocument);
}
