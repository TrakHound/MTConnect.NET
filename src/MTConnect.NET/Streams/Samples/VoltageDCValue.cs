// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// The measurement of the electrical potential between two points in an electrical circuit in which the current is unidirectional.
    /// </summary>
    public class VoltageDCValue : SampleValue
    {
        public VoltageDCValue(double nativeValue, string nativeUnits = Devices.Samples.VoltageDCDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = Devices.Samples.VoltageDCDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
