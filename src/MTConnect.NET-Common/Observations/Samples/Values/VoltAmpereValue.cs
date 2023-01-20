// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The measurement of the apparent power in an electrical circuit, equal to the product of root-mean-square (RMS) voltage and RMS current (commonly referred to as VA).
    /// </summary>
    public class VoltAmpereValue : SampleValue
    {
        public VoltAmpereValue(double nativeValue, string nativeUnits = VoltAmpereDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = VoltAmpereDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}