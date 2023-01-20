// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The positive rate of change of angular velocity.
    /// </summary>
    public class AngularAccelerationValue : SampleValue
    {
        public AngularAccelerationValue(double nativeValue, string nativeUnits = AngularAccelerationDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = AngularAccelerationDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
