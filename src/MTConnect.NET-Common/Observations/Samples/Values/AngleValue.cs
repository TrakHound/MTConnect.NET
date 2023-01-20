// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The measurement of angular position.
    /// </summary>
    public class AngleValue : SampleValue
    { 
        public AngleValue(double nativeValue, string nativeUnits = AngleDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = AngleDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}