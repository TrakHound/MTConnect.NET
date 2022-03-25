// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The measurement of the rate of change of position of a Component
    /// </summary>
    public class VelocityValue : SampleValue
    {
        public VelocityValue(double nativeValue, string nativeUnits = Devices.Samples.VelocityDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = Devices.Samples.VelocityDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
