// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// The volumetric mass of a material per unit volume of that material.
    /// </summary>
    public class DensityValue : SampleValue
    {
        public DensityValue(double nativeValue, string nativeUnits = Devices.Samples.DensityDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = Devices.Samples.DensityDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
