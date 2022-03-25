// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The positive rate of change of velocity.
    /// </summary>
    public class FlowValue : SampleValue
    {
        public FlowValue(double nativeValue, string nativeUnits = Devices.Samples.FlowDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = Devices.Samples.FlowDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
