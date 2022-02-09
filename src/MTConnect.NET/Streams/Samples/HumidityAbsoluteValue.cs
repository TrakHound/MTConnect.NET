// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// The amount of water vapor expressed in grams per cubic meter.
    /// </summary>
    public class HumidityAbsoluteValue : SampleValue
    {
        public HumidityAbsoluteValue(double nativeValue, string nativeUnits = Devices.Samples.HumidityAbsoluteDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = Devices.Samples.HumidityAbsoluteDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
