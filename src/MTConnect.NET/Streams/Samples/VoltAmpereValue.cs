// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// The measurement of the apparent power in an electrical circuit, equal to the product of root-mean-square (RMS) voltage and RMS current (commonly referred to as VA).
    /// </summary>
    public class VoltAmpereValue : SampleValue
    {
        protected override string MetricUnits => "VOLT";
        protected override string InchUnits => "VOLT";


        public VoltAmpereValue(double voltAmpere)
        {
            Value = voltAmpere;
        }
    }
}
