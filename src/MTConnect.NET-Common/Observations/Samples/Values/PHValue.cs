// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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