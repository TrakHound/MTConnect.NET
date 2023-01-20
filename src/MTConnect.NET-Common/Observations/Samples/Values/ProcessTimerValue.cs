// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.DataItems.Samples;
using System;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The measurement of the amount of time a piece of equipment has performed different types of activities associated with the process being performed at that piece of equipment.
    /// </summary>
    public class ProcessTimerValue : SampleValue
    {
        public TimeSpan TimeSpan => TimeSpan.FromSeconds(Value.ToDouble());


        public ProcessTimerValue(TimeSpan accumulatedTime)
        {
            Value = accumulatedTime.TotalSeconds;
            _units = ProcessTimerDataItem.DefaultUnits;
            _nativeUnits = ProcessTimerDataItem.DefaultUnits;
        }


        public ProcessTimerValue AddMilliseconds(double milliseconds)
        {
            Value = Value + (milliseconds / 1000);

            return this;
        }
    }
}
