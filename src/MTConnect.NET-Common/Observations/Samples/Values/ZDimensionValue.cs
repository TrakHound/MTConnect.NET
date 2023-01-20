// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// Measured dimension of an entity relative to the Z direction of the referenced coordinate system.
    /// </summary>
    public class ZDimensionValue : SampleValue
    {
        public ZDimensionValue(double nativeValue, string nativeUnits = ZDimensionDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = ZDimensionDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}