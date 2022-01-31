// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// The measurement of an electric current flowing in one direction only.
    /// </summary>
    public class AmperageDCValue : SampleValue
    {
        protected override string MetricUnits => "AMPERE";
        protected override string InchUnits => "AMPERE";


        public AmperageDCValue(double amperage)
        {
            Value = amperage;
        }
    }
}
