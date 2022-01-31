// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// The measurement of the rate of change of position of a Component
    /// </summary>
    public class VelocityValue : SampleValue
    {
        protected override double MetricConversion => 25.4;
        protected override double InchConversion => 0.03937008;
        protected override string MetricUnits => "MILLIMETER/SECOND";
        protected override string InchUnits => "INCH/SECOND";


        public VelocityValue(double velocity, UnitSystem unitSystem = UnitSystem.METRIC)
        {
            Value = velocity;
            UnitSystem = unitSystem;
        }
    }
}
