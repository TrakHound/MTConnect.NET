// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The measurement of power flowing through or dissipated by an electrical circuit or piece of equipment.
    /// </summary>
    public class WattageValue : SampleValue
    {
        public WattageValue(double nativeValue, string nativeUnits = WattageDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = WattageDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}