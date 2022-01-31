// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// The measurement of the rate of change of angular position.
    /// </summary>
    public class AngularVelocityValue : SampleValue
    {
        protected override string MetricUnits => "DEGREE/SECOND";
        protected override string InchUnits => "DEGREE/SECOND";


        public AngularVelocityValue(double velocity)
        {
            Value = velocity;
        }
    }
}
