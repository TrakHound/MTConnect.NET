// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// A Force applied to a mass in one direction only
    /// </summary>
    public class LinearForceValue : SampleValue
    {
        public LinearForceValue(double nativeValue, string nativeUnits = LinearForceDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = LinearForceDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
