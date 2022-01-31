// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// The measurement of the mass of an object (s) or an amount of material.
    /// </summary>
    public class MassValue : SampleValue
    {
        protected override double MetricConversion => 0.45359237;
        protected override double InchConversion => 2.20462262;
        protected override string MetricUnits => "KILOGRAM";
        protected override string InchUnits => "POUND";


        public MassValue(double mass, UnitSystem unitSystem = UnitSystem.METRIC)
        {
            Value = mass;
            UnitSystem = unitSystem;
        }
    }
}
