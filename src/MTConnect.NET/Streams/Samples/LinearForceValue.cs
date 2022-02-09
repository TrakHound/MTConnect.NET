// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// A Force applied to a mass in one direction only
    /// </summary>
    public class LinearForceValue : SampleValue
    {
        public LinearForceValue(double nativeValue, string nativeUnits = Devices.Samples.LinearForceDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = Devices.Samples.LinearForceDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
