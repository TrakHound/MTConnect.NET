// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// Negative rate of change of velocity.
    /// </summary>
    public class DecelerationValue : SampleValue
    {
        public DecelerationValue(double nativeValue, string nativeUnits = Devices.Samples.DecelerationDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = Devices.Samples.DecelerationDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
