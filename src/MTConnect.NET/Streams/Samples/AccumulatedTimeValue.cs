// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace MTConnect.Streams.Samples
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
            _units = Devices.Samples.AccumulatedTimeDataItem.DefaultUnits;
            _nativeUnits = Devices.Samples.AccumulatedTimeDataItem.DefaultUnits;
        }


        public AccumulatedTimeValue AddMilliseconds(double milliseconds)
        {
            Value = Value + (milliseconds / 1000);

            return this;
        }
    }
}
