// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The measurement of a force that stretches or elongates an object
    /// </summary>
    public class TensionValue : SampleValue
    {
        public TensionValue(double nativeValue, string nativeUnits = TensionDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = TensionDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
