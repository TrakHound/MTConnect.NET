// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// The positive rate of change of velocity.
    /// </summary>
    public class FlowValue : SampleValue
    {
        protected override double MetricConversion => 3.785411789132;
        protected override double InchConversion => 0.2641720524;
        protected override string MetricUnits => "LITER/SECOND";
        protected override string InchUnits => "GALLON/SECOND";


        public FlowValue() { }

        public FlowValue(double flow, UnitSystem unitSystem = UnitSystem.METRIC)
        {
            Value = flow;
            UnitSystem = unitSystem;
        }
    }
}
