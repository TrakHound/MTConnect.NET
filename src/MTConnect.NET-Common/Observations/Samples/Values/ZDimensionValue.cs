// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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
