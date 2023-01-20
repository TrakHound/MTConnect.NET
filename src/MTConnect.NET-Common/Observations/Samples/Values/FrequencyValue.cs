// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The measurement of the number of occurrences of a repeating event per unit time.
    /// </summary>
    public class FrequencyValue : SampleValue
    {
        public FrequencyValue(double nativeValue, string nativeUnits = FrequencyDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = FrequencyDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
