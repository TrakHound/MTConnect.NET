// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The measurement of angular position.
    /// </summary>
    public class DiameterValue : SampleValue
    {
        public DiameterValue(double nativeValue, string nativeUnits = DiameterDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = DiameterDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
