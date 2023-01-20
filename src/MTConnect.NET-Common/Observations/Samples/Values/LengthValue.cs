// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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