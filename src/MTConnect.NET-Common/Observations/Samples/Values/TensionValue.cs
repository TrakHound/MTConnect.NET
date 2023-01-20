// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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