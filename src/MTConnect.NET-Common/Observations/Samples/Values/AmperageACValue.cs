// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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
