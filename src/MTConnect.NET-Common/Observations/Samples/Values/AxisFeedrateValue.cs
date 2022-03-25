// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The measurement of the feedrate of a linear axis.
    /// </summary>
    public class AxisFeedrateValue : SampleValue
    {
        public AxisFeedrateValue(double nativeValue, string nativeUnits = Devices.Samples.AxisFeedrateDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = Devices.Samples.AxisFeedrateDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
