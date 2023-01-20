// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The force per unit area measured relative to a vacuum.
    /// </summary>
    public class PressureAbsoluteValue : SampleValue
    {
        public PressureAbsoluteValue(double nativeValue, string nativeUnits = PressureAbsoluteDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = PressureAbsoluteDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}