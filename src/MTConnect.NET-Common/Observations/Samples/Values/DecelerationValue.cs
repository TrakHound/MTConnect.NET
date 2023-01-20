// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// Negative rate of change of velocity.
    /// </summary>
    public class DecelerationValue : SampleValue
    {
        public DecelerationValue(double nativeValue, string nativeUnits = DecelerationDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = DecelerationDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}