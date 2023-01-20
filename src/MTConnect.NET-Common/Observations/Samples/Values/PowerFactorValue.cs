// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The measurement of the ratio of real power flowing to a load to the apparent power in that AC circuit.
    /// </summary>
    public class PowerFactorValue : SampleValue
    {
        public PowerFactorValue(double nativeValue, string nativeUnits = PowerFactorDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = PowerFactorDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
