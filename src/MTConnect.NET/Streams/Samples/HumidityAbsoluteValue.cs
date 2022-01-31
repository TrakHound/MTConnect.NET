// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// The amount of water vapor expressed in grams per cubic meter.
    /// </summary>
    public class HumidityAbsoluteValue : SampleValue
    {
        protected override double MetricConversion => 27679904.7102672;
        protected override double InchConversion => 0.000000036127292;
        protected override string MetricUnits => "GRAM/CUBIC_METER";
        protected override string InchUnits => "POUND/CUBIC_INCH";


        public HumidityAbsoluteValue(double humidity, UnitSystem unitSystem = UnitSystem.METRIC)
        {
            Value = humidity;
            UnitSystem = unitSystem;
        }
    }
}
