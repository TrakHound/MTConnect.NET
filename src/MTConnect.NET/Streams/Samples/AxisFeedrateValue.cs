// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// The measurement of the feedrate of a linear axis.
    /// </summary>
    public class AxisFeedrateValue : SampleValue
    {
        protected override double MetricConversion => 25.4;
        protected override double InchConversion => 0.03937008;
        protected override string MetricUnits => "MILLIMETER/SECOND";
        protected override string InchUnits => "INCH/SECOND";


        public AxisFeedrateValue(double feedrate, UnitSystem unitSystem = UnitSystem.METRIC)
        {
            Value = feedrate;
            UnitSystem = unitSystem;
        }
    }
}
