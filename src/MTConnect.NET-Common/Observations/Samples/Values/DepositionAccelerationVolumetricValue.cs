// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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
