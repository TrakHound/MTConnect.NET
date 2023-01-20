// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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