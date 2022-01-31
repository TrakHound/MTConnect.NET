// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// The fluid volume of an object or container.
    /// </summary>
    public class VolumeFluidValue : SampleValue
    {
        protected override double MetricConversion => 1;
        protected override double InchConversion => 1;
        protected override string MetricUnits => "MILLILITER";
        protected override string InchUnits => "MILLILITER";


        public VolumeFluidValue(double volume, UnitSystem unitSystem = UnitSystem.METRIC)
        {
            Value = volume;
            UnitSystem = unitSystem;
        }
    }
}
