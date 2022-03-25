// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The measurement of power flowing through or dissipated by an electrical circuit or piece of equipment.
    /// </summary>
    public class WattageValue : SampleValue
    {
        public WattageValue(double nativeValue, string nativeUnits = Devices.Samples.WattageDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = Devices.Samples.WattageDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
