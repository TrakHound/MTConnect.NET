// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// The geometric volume of an object or container.
    /// </summary>
    public class VolumeSpatialValue : SampleValue
    {
        public VolumeSpatialValue(double nativeValue, string nativeUnits = Devices.Samples.VolumeSpatialDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = Devices.Samples.VolumeSpatialDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
