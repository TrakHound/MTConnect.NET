// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The measurement of angular displacement.
    /// </summary>
    public class TiltValue : SampleValue
    {
        public TiltValue(double nativeValue, string nativeUnits = TiltDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = TiltDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}