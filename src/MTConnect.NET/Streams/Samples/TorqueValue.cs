// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// The measurement of the turning force exerted on an object or by an object.
    /// </summary>
    public class TorqueValue : SampleValue
    {
        public TorqueValue(double nativeValue, string nativeUnits = Devices.Samples.TorqueDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = Devices.Samples.TorqueDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
