// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// The change of pressure per unit time.
    /// </summary>
    public class PressurizationRateValue : SampleValue
    {
        protected override double MetricConversion => 1;
        protected override double InchConversion => 1;
        protected override string MetricUnits => "PASCAL/SECOND";
        protected override string InchUnits => "PASCAL/SECOND";


        public PressurizationRateValue(double rate, UnitSystem unitSystem = UnitSystem.METRIC)
        {
            Value = rate;
            UnitSystem = unitSystem;
        }
    }
}
