// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The measurement of a sound level or sound pressure level relative to atmospheric pressure.
    /// </summary>
    public class SoundLevelValue : SampleValue
    {
        public SoundLevelValue(double nativeValue, string nativeUnits = Devices.Samples.SoundLevelDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = Devices.Samples.SoundLevelDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
