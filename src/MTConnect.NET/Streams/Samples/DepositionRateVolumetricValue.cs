// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// The rate at which a spatial volume of material is deposited in an additive manufacturing process.
    /// </summary>
    public class DepositionRateVolumetricValue : SampleValue
    {
        protected override double MetricConversion => 1;
        protected override double InchConversion => 1;
        protected override string MetricUnits => "CUBIC_MILLIMETER/SECOND";
        protected override string InchUnits => "CUBIC_MILLIMETER/SECOND";


        public DepositionRateVolumetricValue(double rate, UnitSystem unitSystem = UnitSystem.METRIC)
        {
            Value = rate;
            UnitSystem = unitSystem;
        }
    }
}
