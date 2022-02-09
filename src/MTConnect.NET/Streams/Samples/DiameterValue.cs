// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// The measurement of angular position.
    /// </summary>
    public class DiameterValue : SampleValue
    {
        public DiameterValue(double nativeValue, string nativeUnits = Devices.Samples.DiameterDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = Devices.Samples.DiameterDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
