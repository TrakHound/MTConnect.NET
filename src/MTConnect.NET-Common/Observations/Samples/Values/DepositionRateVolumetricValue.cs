// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The rate at which a spatial volume of material is deposited in an additive manufacturing process.
    /// </summary>
    public class DepositionRateVolumetricValue : SampleValue
    {
        public DepositionRateVolumetricValue(double nativeValue, string nativeUnits = Devices.Samples.DepositionRateVolumetricDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = Devices.Samples.DepositionRateVolumetricDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
