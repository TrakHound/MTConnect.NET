// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The ratio of the water vapor present over the total weight of the water vapor and air present expressed as a percent.
    /// </summary>
    public class HumiditySpecificValue : SampleValue
    {
        public HumiditySpecificValue(double nativeValue, string nativeUnits = Devices.Samples.HumiditySpecificDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = Devices.Samples.HumiditySpecificDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
