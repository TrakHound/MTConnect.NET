// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The speed difference (relative velocity) between the cutting mechanism and the surface of the workpiece it is operating on.
    /// </summary>
    public class CuttingSpeedValue : SampleValue
    {
        public CuttingSpeedValue(double nativeValue, string nativeUnits = CuttingSpeedDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = CuttingSpeedDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}