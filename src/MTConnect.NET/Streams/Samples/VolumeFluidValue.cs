// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// The fluid volume of an object or container.
    /// </summary>
    public class VolumeFluidValue : SampleValue
    {
        public VolumeFluidValue(double nativeValue, string nativeUnits = Devices.Samples.VolumeFluidDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = Devices.Samples.VolumeFluidDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
