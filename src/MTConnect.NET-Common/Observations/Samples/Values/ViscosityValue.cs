// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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
