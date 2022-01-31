// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// The measurement of an electrical current that reverses direction at regular short intervals.
    /// </summary>
    public class AmperageACValue : SampleValue
    {
        protected override string MetricUnits => "AMPERE";
        protected override string InchUnits => "AMPERE";


        public AmperageACValue() { }

        public AmperageACValue(double amperage)
        {
            Value = amperage;
        }
    }
}
