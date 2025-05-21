// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using System;

namespace MTConnect.Observations.Output
{
    public interface IObservationOutput
    {
        string DeviceUuid { get; }

        IDataItem DataItem { get; }

        string DataItemId { get; }

        DataItemCategory Category { get; }

        string Type { get; }

        string SubType { get; }

        string Name { get; }

        ulong InstanceId { get; }

        ulong Sequence { get; }

        DateTime Timestamp { get; }

        DateTimeOffset TimeZoneTimestamp { get; }

        string CompositionId { get; }

        DataItemRepresentation Representation { get; }

        Quality Quality { get; }

        bool Deprecated { get; }

        bool Extended { get; }

        ObservationValue[] Values { get; }


        string GetValue(string valueKey);
    }
}