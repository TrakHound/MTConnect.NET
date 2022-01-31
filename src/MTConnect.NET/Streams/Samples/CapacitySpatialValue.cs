// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// The geometric capacity of an object or container.
    /// </summary>
    public class CapacitySpatialValue : SampleValue
    {
        protected override double MetricConversion => 16387.064;
        protected override double InchConversion => 0.0000610237;
        protected override string MetricUnits => "CUBIC_MILLIMETER";
        protected override string InchUnits => "CUBIC_INCH";


        public CapacitySpatialValue(double capacity, UnitSystem unitSystem = UnitSystem.METRIC)
        {
            Value = capacity;
            UnitSystem = unitSystem;
        }
    }
}
