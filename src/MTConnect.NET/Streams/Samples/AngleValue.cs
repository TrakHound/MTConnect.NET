// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// The measurement of angular position.
    /// </summary>
    public class AngleValue : SampleValue
    {
        protected override string MetricUnits => "DEGREE";
        protected override string InchUnits => "DEGREE";


        public AngleValue() { }

        public AngleValue(double degrees)
        {
            Value = degrees;
        }
    }
}
