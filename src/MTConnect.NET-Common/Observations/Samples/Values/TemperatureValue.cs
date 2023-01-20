// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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
