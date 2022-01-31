// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// The measurement of the amount of time a piece of equipment has performed different types of activities associated with the process being performed at that piece of equipment.
    /// </summary>
    public class ProcessTimerValue : SampleValue
    {
        protected override string MetricUnits => "SECOND";
        protected override string InchUnits => "SECOND";


        public ProcessTimerValue(TimeSpan accumulatedTime)
        {
            Value = accumulatedTime.TotalSeconds;
        }
    }
}
