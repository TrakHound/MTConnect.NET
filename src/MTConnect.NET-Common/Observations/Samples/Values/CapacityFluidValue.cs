// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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