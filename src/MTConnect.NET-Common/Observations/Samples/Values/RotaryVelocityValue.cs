// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The measurement of the rotational speed of a rotary axis.
    /// </summary>
    public class RotaryVelocityValue : SampleValue
    {
        public RotaryVelocityValue(double nativeValue, string nativeUnits = RotaryVelocityDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = RotaryVelocityDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}