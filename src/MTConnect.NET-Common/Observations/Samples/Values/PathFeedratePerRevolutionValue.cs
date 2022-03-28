// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The feedrate for the axes, or a single axis.
    /// </summary>
    public class PathFeedratePerRevolutionValue : SampleValue
    {
        public PathFeedratePerRevolutionValue(double nativeValue, string nativeUnits = PathFeedratePerRevolutionDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = PathFeedratePerRevolutionDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
