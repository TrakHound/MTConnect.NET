// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// The measurement of the amount of a substance remaining compared to the planned maximum amount of that substance.
    /// </summary>
    public class FillLevelValue : SampleValue
    {
        public FillLevelValue(double nativeValue, string nativeUnits = Devices.Samples.FillLevelDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = Devices.Samples.FillLevelDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
