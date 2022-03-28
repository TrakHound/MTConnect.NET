// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// A measure of the acidity or alkalinity of a solution
    /// </summary>
    public class PHValue : SampleValue
    {
        public PHValue(double nativeValue, string nativeUnits = PHDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = PHDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
