// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The positive rate of change of velocity.
    /// </summary>
    public class AccelerationValue : SampleValue
    {
        public AccelerationValue(double nativeValue, string nativeUnits = AccelerationDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = AccelerationDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
