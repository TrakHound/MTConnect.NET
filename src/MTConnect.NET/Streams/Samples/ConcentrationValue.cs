// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// The measurement of the percentage of one component within a mixture of components
    /// </summary>
    public class ConcentrationValue : SampleValue
    {
        protected override string MetricUnits => "PERCENT";
        protected override string InchUnits => "PERCENT";


        public ConcentrationValue(double percent)
        {
            Value = percent;
        }
    }
}
