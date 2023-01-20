// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The measurement of the change in position of an object.
    /// </summary>
    public class DisplacementValue : SampleValue
    {
        public DisplacementValue(double nativeValue, string nativeUnits = DisplacementDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = DisplacementDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}