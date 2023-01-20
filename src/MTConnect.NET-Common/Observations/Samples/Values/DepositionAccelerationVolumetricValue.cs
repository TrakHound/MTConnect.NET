// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The rate of change in spatial volume of material deposited in an additive manufacturing process.
    /// </summary>
    public class DepositionAccelerationVolumetricValue : SampleValue
    {
        public DepositionAccelerationVolumetricValue(double nativeValue, string nativeUnits = DepositionAccelerationVolumetricDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = DepositionAccelerationVolumetricDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}