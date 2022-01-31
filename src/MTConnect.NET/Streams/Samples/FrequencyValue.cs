// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// The measurement of the number of occurrences of a repeating event per unit time.
    /// </summary>
    public class FrequencyValue : SampleValue
    {
        protected override string MetricUnits => "HERTZ";
        protected override string InchUnits => "HERTZ";


        public FrequencyValue(double hertz)
        {
            Value = hertz;
        }
    }
}
