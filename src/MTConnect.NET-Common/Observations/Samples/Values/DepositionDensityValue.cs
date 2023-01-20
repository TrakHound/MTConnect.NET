// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The density of the material deposited in an additive manufacturing process per unit of volume.
    /// </summary>
    public class DepositionDensityValue : SampleValue
    {
        public DepositionDensityValue(double nativeValue, string nativeUnits = DepositionDensityDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = DepositionDensityDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}