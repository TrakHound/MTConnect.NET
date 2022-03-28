// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The measurement of the ability of a material to conduct electricity.
    /// </summary>
    public class ConductivityValue : SampleValue
    {
        public ConductivityValue(double nativeValue, string nativeUnits = ConductivityDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = ConductivityDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
