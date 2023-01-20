// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// Measured dimension of an entity relative to the Y direction of the referenced coordinate system.
    /// </summary>
    public class YDimensionValue : SampleValue
    {
        public YDimensionValue(double nativeValue, string nativeUnits = YDimensionDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = YDimensionDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}