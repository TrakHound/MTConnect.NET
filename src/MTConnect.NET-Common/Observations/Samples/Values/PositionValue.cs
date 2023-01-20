// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// A measured or calculated position of a Component element as reported by a piece of equipment.
    /// </summary>
    public class PositionValue : SampleValue
    {
        public PositionValue(double nativeValue, string nativeUnits = PositionDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = PositionDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}