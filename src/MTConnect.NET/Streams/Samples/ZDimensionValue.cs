// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// Measured dimension of an entity relative to the Z direction of the referenced coordinate system.
    /// </summary>
    public class ZDimensionValue : SampleValue
    {
        public ZDimensionValue(double nativeValue, string nativeUnits = Devices.Samples.ZDimensionDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = Devices.Samples.ZDimensionDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
