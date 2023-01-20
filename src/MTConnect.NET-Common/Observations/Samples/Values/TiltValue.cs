// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The measurement of angular displacement.
    /// </summary>
    public class TiltValue : SampleValue
    {
        public TiltValue(double nativeValue, string nativeUnits = TiltDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = TiltDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
