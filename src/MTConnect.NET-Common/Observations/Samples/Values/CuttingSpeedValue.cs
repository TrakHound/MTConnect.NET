// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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
