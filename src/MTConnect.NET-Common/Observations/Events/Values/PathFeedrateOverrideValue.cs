// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events.Values
{
    /// <summary>
    /// The value of a signal or calculation issued to adjust the feedrate for the axes associated with a Path component that may represent a single axis or the coordinated movement of multiple axes.
    /// </summary>
    public class PathFeedrateOverrideValue : EventValue
    {
        public PathFeedrateOverrideValue() { }

        public PathFeedrateOverrideValue(double percent)
        {
            Value = percent;
        }
    }
}