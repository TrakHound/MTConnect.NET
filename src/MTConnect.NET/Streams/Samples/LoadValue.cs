// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// The measurement of the actual versus the standard rating of a piece of equipment.
    /// </summary>
    public class LoadValue : SampleValue
    {
        protected override string MetricUnits => "PERCENT";
        protected override string InchUnits => "PERCENT";


        public LoadValue() { }

        public LoadValue(double percent)
        {
            Value = percent;
        }
    }
}
