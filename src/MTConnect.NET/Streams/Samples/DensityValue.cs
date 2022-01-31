// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// The volumetric mass of a material per unit volume of that material.
    /// </summary>
    public class DensityValue : SampleValue
    {
        protected override double MetricConversion => 1;
        protected override double InchConversion => 1;
        protected override string MetricUnits => "MILLIGRAM/CUBIC_MILLIMETER";
        protected override string InchUnits => "MILLIGRAM/CUBIC_MILLIMETER";


        public DensityValue(double density, UnitSystem unitSystem = UnitSystem.METRIC)
        {
            Value = density;
            UnitSystem = unitSystem;
        }
    }
}
