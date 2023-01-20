// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The measurement of the electrical potential between two points in an electrical circuit in which the current is unidirectional.
    /// </summary>
    public class VoltageDCValue : SampleValue
    {
        public VoltageDCValue(double nativeValue, string nativeUnits = VoltageDCDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = VoltageDCDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}