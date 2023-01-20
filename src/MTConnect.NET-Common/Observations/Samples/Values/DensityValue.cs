// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The volumetric mass of a material per unit volume of that material.
    /// </summary>
    public class DensityValue : SampleValue
    {
        public DensityValue(double nativeValue, string nativeUnits = DensityDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = DensityDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}