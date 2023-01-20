// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The positive rate of change of velocity.
    /// </summary>
    public class FlowValue : SampleValue
    {
        public FlowValue(double nativeValue, string nativeUnits = FlowDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = FlowDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}