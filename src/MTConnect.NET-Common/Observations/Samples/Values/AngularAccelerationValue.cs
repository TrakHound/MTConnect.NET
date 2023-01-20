// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The positive rate of change of angular velocity.
    /// </summary>
    public class AngularAccelerationValue : SampleValue
    {
        public AngularAccelerationValue(double nativeValue, string nativeUnits = AngularAccelerationDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = AngularAccelerationDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}