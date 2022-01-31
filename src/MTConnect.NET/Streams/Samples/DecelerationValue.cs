// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// Negative rate of change of velocity.
    /// </summary>
    public class DecelerationValue : SampleValue
    {
        protected override double MetricConversion => 25.4;
        protected override double InchConversion => 0.03937008;
        protected override string MetricUnits => "MILLIMETER/SECOND^2";
        protected override string InchUnits => "INCH/SECOND^2";


        public DecelerationValue(double acceleration, UnitSystem unitSystem = UnitSystem.METRIC)
        {
            Value = acceleration;
            UnitSystem = unitSystem;
        }
    }
}
