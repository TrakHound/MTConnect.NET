// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// The force per unit area measured relative to atmospheric pressure.
    /// </summary>
    public class PressureValue : SampleValue
    {
        public PressureValue(double nativeValue, string nativeUnits = Devices.Samples.PressureDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = Devices.Samples.PressureDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
