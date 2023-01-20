// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The amount of water vapor expressed in grams per cubic meter.
    /// </summary>
    public class HumidityAbsoluteValue : SampleValue
    {
        public HumidityAbsoluteValue(double nativeValue, string nativeUnits = HumidityAbsoluteDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = HumidityAbsoluteDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}