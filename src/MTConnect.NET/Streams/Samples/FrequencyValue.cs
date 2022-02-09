// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// The measurement of the number of occurrences of a repeating event per unit time.
    /// </summary>
    public class FrequencyValue : SampleValue
    {
        public FrequencyValue(double nativeValue, string nativeUnits = Devices.Samples.FrequencyDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = Devices.Samples.FrequencyDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
