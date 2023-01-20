// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The measurement of reactive power in an AC electrical circuit(commonly referred to as VAR).
    /// </summary>
    public class VoltAmpereReactiveValue : SampleValue
    {
        public VoltAmpereReactiveValue(double nativeValue, string nativeUnits = VoltAmpereReactiveDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = VoltAmpereReactiveDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
