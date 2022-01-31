// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// The feedrate for the axes, or a single axis.
    /// </summary>
    public class PathFeedratePerRevolutionValue : SampleValue
    {
        protected override double MetricConversion => 25.4;
        protected override double InchConversion => 0.03937008;
        protected override string MetricUnits => "MILLIMETER/REVOLUTION";
        protected override string InchUnits => "INCH/REVOLUTION";


        public PathFeedratePerRevolutionValue(double feedrate, UnitSystem unitSystem = UnitSystem.METRIC)
        {
            Value = feedrate;
            UnitSystem = unitSystem;
        }
    }
}
