// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The measurement of the feedrate for the axes, or a single axis, associated with a Path component-a vector.
    /// </summary>
    public class PathFeedrateValue : SampleValue
    {
        public PathFeedrateValue(double nativeValue, string nativeUnits = PathFeedrateDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = PathFeedrateDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}