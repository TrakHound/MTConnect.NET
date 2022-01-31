// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// The fluid capacity of an object or container.
    /// </summary>
    public class CapacityFluidValue : SampleValue
    {
        protected override string MetricUnits => "MILLILITER";
        protected override string InchUnits => "MILLILITER";


        public CapacityFluidValue() { }

        public CapacityFluidValue(double capacity)
        {
            Value = capacity;
        }
    }
}
