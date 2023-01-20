// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// Measured dimension of an entity relative to the X direction of the referenced coordinate system.
    /// </summary>
    public class XDimensionValue : SampleValue
    {
        public XDimensionValue(double nativeValue, string nativeUnits = XDimensionDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = XDimensionDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
