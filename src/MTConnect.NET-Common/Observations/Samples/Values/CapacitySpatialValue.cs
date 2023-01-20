// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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