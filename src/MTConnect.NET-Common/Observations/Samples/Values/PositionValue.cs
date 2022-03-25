// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// A measured or calculated position of a Component element as reported by a piece of equipment.
    /// </summary>
    public class PositionValue : SampleValue
    {
        public PositionValue(double nativeValue, string nativeUnits = Devices.Samples.PositionDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = Devices.Samples.PositionDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
