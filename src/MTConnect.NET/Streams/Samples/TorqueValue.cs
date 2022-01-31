// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// The measurement of the turning force exerted on an object or by an object.
    /// </summary>
    public class TorqueValue : SampleValue
    {
        protected override double MetricConversion => 0.1129846742;
        protected override double InchConversion => 8.850757916;
        protected override string MetricUnits => "NEWTON_METER";
        protected override string InchUnits => "INCH_POUND";


        public TorqueValue(double torque, UnitSystem unitSystem = UnitSystem.METRIC)
        {
            Value = torque;
            UnitSystem = unitSystem;
        }
    }
}
