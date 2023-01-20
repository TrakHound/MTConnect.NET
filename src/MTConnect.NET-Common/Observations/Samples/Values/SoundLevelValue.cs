// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The measurement of a sound level or sound pressure level relative to atmospheric pressure.
    /// </summary>
    public class SoundLevelValue : SampleValue
    {
        public SoundLevelValue(double nativeValue, string nativeUnits = SoundLevelDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = SoundLevelDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}