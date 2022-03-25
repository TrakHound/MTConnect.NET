// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The measurement of angular displacement.
    /// </summary>
    public class TiltValue : SampleValue
    {
        public TiltValue(double nativeValue, string nativeUnits = Devices.Samples.TiltDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = Devices.Samples.TiltDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
