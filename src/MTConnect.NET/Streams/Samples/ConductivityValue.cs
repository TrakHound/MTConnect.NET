// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// The measurement of the ability of a material to conduct electricity.
    /// </summary>
    public class ConductivityValue : SampleValue
    {
        protected override double MetricConversion => 1;
        protected override double InchConversion => 1;
        protected override string MetricUnits => "SIEMENS/METER";
        protected override string InchUnits => "SIEMENS/METER";


        public ConductivityValue(double conductivity, UnitSystem unitSystem = UnitSystem.METRIC)
        {
            Value = conductivity;
            UnitSystem = unitSystem;
        }
    }
}
