// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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
