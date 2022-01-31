// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// The rate of change in spatial volume of material deposited in an additive manufacturing process.
    /// </summary>
    public class DepositionAccelerationVolumetricValue : SampleValue
    {
        protected override double MetricConversion => 1;
        protected override double InchConversion => 1;
        protected override string MetricUnits => "CUBIC_MILLIMETER/SECOND^2";
        protected override string InchUnits => "CUBIC_MILLIMETER/SECOND^2";


        public DepositionAccelerationVolumetricValue(double acceleration, UnitSystem unitSystem = UnitSystem.METRIC)
        {
            Value = acceleration;
            UnitSystem = unitSystem;
        }
    }
}
