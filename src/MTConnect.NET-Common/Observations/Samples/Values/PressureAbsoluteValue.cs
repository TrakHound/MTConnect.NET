// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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
