// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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