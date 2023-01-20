// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The measurement of the rate of change of angular position.
    /// </summary>
    public class AngularVelocityValue : SampleValue
    {
        public AngularVelocityValue(double nativeValue, string nativeUnits = AngularVelocityDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = AngularVelocityDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}