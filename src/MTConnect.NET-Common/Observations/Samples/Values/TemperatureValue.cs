// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The measurement of temperature.
    /// </summary>
    public class TemperatureValue : SampleValue
    {
        public TemperatureValue(double nativeValue, string nativeUnits = TemperatureDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = TemperatureDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}