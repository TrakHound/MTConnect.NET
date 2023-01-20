// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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