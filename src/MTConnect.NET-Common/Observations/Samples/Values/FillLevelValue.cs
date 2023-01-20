// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The measurement of the amount of a substance remaining compared to the planned maximum amount of that substance.
    /// </summary>
    public class FillLevelValue : SampleValue
    {
        public FillLevelValue(double nativeValue, string nativeUnits = FillLevelDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = FillLevelDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}