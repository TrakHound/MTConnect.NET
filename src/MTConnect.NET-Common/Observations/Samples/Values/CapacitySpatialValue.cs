// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The geometric capacity of an object or container.
    /// </summary>
    public class CapacitySpatialValue : SampleValue
    {
        public CapacitySpatialValue(double nativeValue, string nativeUnits = CapacitySpatialDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = CapacitySpatialDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
