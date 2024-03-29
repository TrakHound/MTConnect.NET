// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The measurement of accumulated time for an activity or event.
    /// </summary>
    public class AccumulatedTimeValue : SampleValue
    {
        public TimeSpan TimeSpan => TimeSpan.FromSeconds(Value.ToDouble());

        public AccumulatedTimeValue(TimeSpan accumulatedTime)
        {
            Value = accumulatedTime.TotalSeconds;
            _units = AccumulatedTimeDataItem.DefaultUnits;
            _nativeUnits = AccumulatedTimeDataItem.DefaultUnits;
        }


        public AccumulatedTimeValue AddMilliseconds(double milliseconds)
        {
            Value = Value + (milliseconds / 1000);

            return this;
        }
    }
}