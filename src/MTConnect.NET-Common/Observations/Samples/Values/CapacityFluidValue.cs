// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The fluid capacity of an object or container.
    /// </summary>
    public class CapacityFluidValue : SampleValue
    {
        public CapacityFluidValue(double nativeValue, string nativeUnits = Devices.Samples.CapacityFluidDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = Devices.Samples.CapacityFluidDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
