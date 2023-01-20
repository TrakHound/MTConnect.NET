// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// Negative rate of change of angular velocity.
    /// </summary>
    public class AngularDecelerationValue : SampleValue
    {
        public AngularDecelerationValue(double nativeValue, string nativeUnits = AngularDecelerationDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = AngularDecelerationDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
