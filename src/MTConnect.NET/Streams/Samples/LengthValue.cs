// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// The measurement of the length of an object.
    /// </summary>
    public class LengthValue : SampleValue
    {
        public LengthValue(double nativeValue, string nativeUnits = Devices.Samples.LengthDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = Devices.Samples.LengthDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
