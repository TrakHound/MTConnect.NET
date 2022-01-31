// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// Negative rate of change of angular velocity.
    /// </summary>
    public class AngularDecelerationValue : SampleValue
    {
        protected override string MetricUnits => "DEGREE/SECOND^2";
        protected override string InchUnits => "DEGREE/SECOND^2";


        public AngularDecelerationValue(double deceleration)
        {
            Value = deceleration;
        }
    }
}
