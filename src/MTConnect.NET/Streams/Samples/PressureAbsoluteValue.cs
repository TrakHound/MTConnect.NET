// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// The force per unit area measured relative to a vacuum.
    /// </summary>
    public class PressureAbsoluteValue : SampleValue
    {
        protected override double MetricConversion => 6894.759086775;
        protected override double InchConversion => 0.0001450377;
        protected override string MetricUnits => "PASCAL";
        protected override string InchUnits => "INCH/SECOND^2";


        public PressureAbsoluteValue(double pressure, UnitSystem unitSystem = UnitSystem.METRIC)
        {
            Value = pressure;
            UnitSystem = unitSystem;
        }
    }
}
