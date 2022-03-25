// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The measurement of the change in position of an object.
    /// </summary>
    public class DisplacementValue : SampleValue
    {
        public DisplacementValue(double nativeValue, string nativeUnits = Devices.Samples.DisplacementDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = Devices.Samples.DisplacementDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
