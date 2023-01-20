// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// Negative rate of change of velocity.
    /// </summary>
    public class DecelerationValue : SampleValue
    {
        public DecelerationValue(double nativeValue, string nativeUnits = DecelerationDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = DecelerationDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
