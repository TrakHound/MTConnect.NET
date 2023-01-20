// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The measurement of an electrical current that reverses direction at regular short intervals.
    /// </summary>
    public class AmperageACValue : SampleValue
    {
        public AmperageACValue(double nativeValue, string nativeUnits = AmperageACDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = AmperageACDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}