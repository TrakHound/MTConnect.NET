// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// The measurement of a fluids resistance to flow
    /// </summary>
    public class ViscosityValue : SampleValue
    {
        protected override double MetricConversion => 1;
        protected override double InchConversion => 1;
        protected override string MetricUnits => "PASCAL_SECOND";
        protected override string InchUnits => "PASCAL_SECOND";


        public ViscosityValue(double viscosity, UnitSystem unitSystem = UnitSystem.METRIC)
        {
            Value = viscosity;
            UnitSystem = unitSystem;
        }
    }
}
