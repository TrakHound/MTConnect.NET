// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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