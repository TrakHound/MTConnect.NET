// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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