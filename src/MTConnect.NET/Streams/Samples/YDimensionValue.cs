// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// Measured dimension of an entity relative to the Y direction of the referenced coordinate system.
    /// </summary>
    public class YDimensionValue : SampleValue
    {
        protected override double MetricConversion => 25.4;
        protected override double InchConversion => 0.03937008;
        protected override string MetricUnits => "MILLIMETER";
        protected override string InchUnits => "INCH";


        public YDimensionValue(double dimension, UnitSystem unitSystem = UnitSystem.METRIC)
        {
            Value = dimension;
            UnitSystem = unitSystem;
        }
    }
}
