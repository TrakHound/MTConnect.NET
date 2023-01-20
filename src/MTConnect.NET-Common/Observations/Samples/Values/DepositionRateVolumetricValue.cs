// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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