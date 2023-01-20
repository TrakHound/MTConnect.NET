// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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