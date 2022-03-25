// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The measurement of reactive power in an AC electrical circuit(commonly referred to as VAR).
    /// </summary>
    public class VoltAmpereReactiveValue : SampleValue
    {
        public VoltAmpereReactiveValue(double nativeValue, string nativeUnits = Devices.Samples.VoltAmpereReactiveDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = Devices.Samples.VoltAmpereReactiveDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
