// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The ratio of the water vapor present over the total weight of the water vapor and air present expressed as a percent.
    /// </summary>
    public class HumiditySpecificValue : SampleValue
    {
        public HumiditySpecificValue(double nativeValue, string nativeUnits = HumiditySpecificDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = HumiditySpecificDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}