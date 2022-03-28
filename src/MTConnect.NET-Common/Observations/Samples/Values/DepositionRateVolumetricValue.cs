// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The rate at which a spatial volume of material is deposited in an additive manufacturing process.
    /// </summary>
    public class DepositionRateVolumetricValue : SampleValue
    {
        public DepositionRateVolumetricValue(double nativeValue, string nativeUnits = DepositionRateVolumetricDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = DepositionRateVolumetricDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
