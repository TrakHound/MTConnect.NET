// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// The measurement of the length of an object.
    /// </summary>
    public class LengthValue : SampleValue
    {
        protected override double MetricConversion => 25.4;
        protected override double InchConversion => 0.03937008;
        protected override string MetricUnits => "MILLIMETER";
        protected override string InchUnits => "INCH";


        public LengthValue(double length, UnitSystem unitSystem = UnitSystem.METRIC)
        {
            Value = length;
            UnitSystem = unitSystem;
        }
    }
}
