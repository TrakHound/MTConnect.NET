// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The measurement of the mass of an object (s) or an amount of material.
    /// </summary>
    public class MassValue : SampleValue
    {
        public MassValue(double nativeValue, string nativeUnits = MassDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = MassDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
