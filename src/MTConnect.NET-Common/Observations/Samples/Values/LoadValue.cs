// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The measurement of the actual versus the standard rating of a piece of equipment.
    /// </summary>
    public class LoadValue : SampleValue
    {
        public LoadValue(double nativeValue, string nativeUnits = LoadDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = LoadDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
