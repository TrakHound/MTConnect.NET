// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// Measured dimension of an entity relative to the X direction of the referenced coordinate system.
    /// </summary>
    public class XDimensionValue : SampleValue
    {
        public XDimensionValue(double nativeValue, string nativeUnits = Devices.Samples.XDimensionDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = Devices.Samples.XDimensionDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
