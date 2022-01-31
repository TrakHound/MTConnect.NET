// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// A measure of the acidity or alkalinity of a solution
    /// </summary>
    public class PHValue : SampleValue
    {
        protected override string MetricUnits => "PH";
        protected override string InchUnits => "PH";


        public PHValue(double ph)
        {
            Value = ph;
        }
    }
}
