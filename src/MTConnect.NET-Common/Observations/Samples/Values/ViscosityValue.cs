// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The measurement of a fluids resistance to flow
    /// </summary>
    public class ViscosityValue : SampleValue
    {
        public ViscosityValue(double nativeValue, string nativeUnits = ViscosityDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = ViscosityDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}