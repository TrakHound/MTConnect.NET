// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// The ratio of the water vapor present over the total weight of the water vapor and air present expressed as a percent.
    /// </summary>
    public class HumiditySpecificValue : SampleValue
    {
        protected override string MetricUnits => "PERCENT";
        protected override string InchUnits => "PERCENT";


        public HumiditySpecificValue(double percent)
        {
            Value = percent;
        }
    }
}
