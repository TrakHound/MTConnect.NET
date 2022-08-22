// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices;
using MTConnect.Devices.DataItems;
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

        long Sequence { get; }

        DateTime Timestamp { get; }

        string CompositionId { get; }

        DataItemRepresentation Representation { get; }

        ObservationValue[] Values { get; }


        string GetValue(string valueKey);
    }
}
