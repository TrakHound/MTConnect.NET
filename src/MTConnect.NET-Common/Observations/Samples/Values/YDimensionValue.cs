// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// Measured dimension of an entity relative to the Y direction of the referenced coordinate system.
    /// </summary>
    public class YDimensionValue : SampleValue
    {
        public YDimensionValue(double nativeValue, string nativeUnits = Devices.Samples.YDimensionDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = Devices.Samples.YDimensionDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
