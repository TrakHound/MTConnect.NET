// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// The amount of water vapor present expressed as a percent to reach saturation at the same temperature.
    /// </summary>
    public class HumidityRelativeValue : SampleValue
    {
        protected override string MetricUnits => "PERCENT";
        protected override string InchUnits => "PERCENT";


        public HumidityRelativeValue(double percent)
        {
            Value = percent;
        }
    }
}
