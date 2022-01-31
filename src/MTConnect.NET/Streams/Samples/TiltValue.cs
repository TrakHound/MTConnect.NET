// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// The measurement of angular displacement.
    /// </summary>
    public class TiltValue : SampleValue
    {
        protected override double MetricConversion => 17452.0069808;
        protected override double InchConversion => 0.0000573;
        protected override string MetricUnits => "MICRO_RADIAN";
        protected override string InchUnits => "DEGREE";


        public TiltValue(double tilt, UnitSystem unitSystem = UnitSystem.METRIC)
        {
            Value = tilt;
            UnitSystem = unitSystem;
        }
    }
}
