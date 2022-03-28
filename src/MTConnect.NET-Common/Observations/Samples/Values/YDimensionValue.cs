// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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
