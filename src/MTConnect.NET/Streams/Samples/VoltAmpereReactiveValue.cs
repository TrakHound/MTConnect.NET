// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// The measurement of reactive power in an AC electrical circuit(commonly referred to as VAR).
    /// </summary>
    public class VoltAmpereReactiveValue : SampleValue
    {
        protected override string MetricUnits => "VOLT_AMPERE_REACTIVE";
        protected override string InchUnits => "VOLT_AMPERE_REACTIVE";


        public VoltAmpereReactiveValue(double voltAmpereReactive)
        {
            Value = voltAmpereReactive;
        }
    }
}
