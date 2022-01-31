// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// The geometric volume of an object or container.
    /// </summary>
    public class VolumeSpatialValue : SampleValue
    {
        protected override double MetricConversion => 16387.075841;
        protected override double InchConversion => 0.0000610237;
        protected override string MetricUnits => "CUBIC_MILLIMETER";
        protected override string InchUnits => "CUBIC_INCH";


        public VolumeSpatialValue(double volume, UnitSystem unitSystem = UnitSystem.METRIC)
        {
            Value = volume;
            UnitSystem = unitSystem;
        }
    }
}
