// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// Negative rate of change of angular velocity.
    /// </summary>
    public class AngularDecelerationValue : SampleValue
    {
        public AngularDecelerationValue(double nativeValue, string nativeUnits = AngularDecelerationDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = AngularDecelerationDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}