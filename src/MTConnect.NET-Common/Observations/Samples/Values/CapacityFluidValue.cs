// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The fluid capacity of an object or container.
    /// </summary>
    public class CapacityFluidValue : SampleValue
    {
        public CapacityFluidValue(double nativeValue, string nativeUnits = CapacityFluidDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = CapacityFluidDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
