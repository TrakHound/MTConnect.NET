// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The amount of water vapor present expressed as a percent to reach saturation at the same temperature.
    /// </summary>
    public class HumidityRelativeValue : SampleValue
    {
        public HumidityRelativeValue(double nativeValue, string nativeUnits = HumidityRelativeDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = HumidityRelativeDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
