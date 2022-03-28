// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The measurement of the rate of change of position of a Component
    /// </summary>
    public class VelocityValue : SampleValue
    {
        public VelocityValue(double nativeValue, string nativeUnits = VelocityDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = VelocityDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
