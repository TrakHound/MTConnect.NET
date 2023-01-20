// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The measurement of the length of an object.
    /// </summary>
    public class LengthValue : SampleValue
    {
        public LengthValue(double nativeValue, string nativeUnits = LengthDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = LengthDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
