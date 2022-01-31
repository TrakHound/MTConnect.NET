// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// The spatial volume of material deposited in an additive manufacturing process.
    /// </summary>
    public class DepositionVolumeValue : SampleValue
    {
        protected override double MetricConversion => 1;
        protected override double InchConversion => 1;
        protected override string MetricUnits => "MILLIGRAM";
        protected override string InchUnits => "MILLIGRAM";


        public DepositionVolumeValue(double volume, UnitSystem unitSystem = UnitSystem.METRIC)
        {
            Value = volume;
            UnitSystem = unitSystem;
        }
    }
}
