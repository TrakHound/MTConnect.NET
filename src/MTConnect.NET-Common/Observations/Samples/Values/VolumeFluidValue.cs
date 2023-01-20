// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The fluid volume of an object or container.
    /// </summary>
    public class VolumeFluidValue : SampleValue
    {
        public VolumeFluidValue(double nativeValue, string nativeUnits = VolumeFluidDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = VolumeFluidDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
