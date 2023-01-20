// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The measurement of the rate of change of position of a Component
    /// </summary>
    public class VelocityValue : SampleValue
    {
        public VelocityValue(double nativeValue, string nativeUnits = VelocityDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = VelocityDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}