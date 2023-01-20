// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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