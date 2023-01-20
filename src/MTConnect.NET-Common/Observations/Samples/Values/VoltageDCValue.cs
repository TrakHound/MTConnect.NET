// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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
