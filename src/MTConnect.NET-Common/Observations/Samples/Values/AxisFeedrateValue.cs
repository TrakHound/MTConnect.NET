// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The measurement of the feedrate of a linear axis.
    /// </summary>
    public class AxisFeedrateValue : SampleValue
    {
        public AxisFeedrateValue(double nativeValue, string nativeUnits = AxisFeedrateDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = AxisFeedrateDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}