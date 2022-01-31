// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// The measurement of temperature.
    /// </summary>
    public class TemperatureValue : SampleValue
    {
        protected override string MetricUnits => "CELSIUS";
        protected override string InchUnits => "FARENHEIT";


        public TemperatureValue() { }

        public TemperatureValue(double temperature, UnitSystem unitSystem = UnitSystem.METRIC)
        {
            Value = temperature;
            UnitSystem = unitSystem;
        }

        public override double ToMetric()
        {
            if (UnitSystem == UnitSystem.INCH)
            {
                double x = Value.ToDouble() - 32;
                double y = (double)5 / (double)9; 
                double z = x * y;
                return z;
                //return (Value.ToDouble() - 32) * (5/9);
            }

            return Value.ToDouble();
        }

        public override double ToInch()
        {
            if (UnitSystem == UnitSystem.METRIC)
            {
                return (Value.ToDouble() * (9/5)) + 32;
            }

            return Value.ToDouble();
        }
    }
}
