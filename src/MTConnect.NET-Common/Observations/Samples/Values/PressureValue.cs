// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The force per unit area measured relative to atmospheric pressure.
    /// </summary>
    public class PressureValue : SampleValue
    {
        public PressureValue(double nativeValue, string nativeUnits = PressureDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = PressureDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}