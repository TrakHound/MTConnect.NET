// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// The measurement of a force that stretches or elongates an object
    /// </summary>
    public class TensionValue : SampleValue
    {
        protected override double MetricConversion => 4.448221615254;
        protected override double InchConversion => 0.2248089431;
        protected override string MetricUnits => "NEWTON";
        protected override string InchUnits => "POUND";


        public TensionValue(double force, UnitSystem unitSystem = UnitSystem.METRIC)
        {
            Value = force;
            UnitSystem = unitSystem;
        }
    }
}
