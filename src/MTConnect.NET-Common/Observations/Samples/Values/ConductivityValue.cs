// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The measurement of the ability of a material to conduct electricity.
    /// </summary>
    public class ConductivityValue : SampleValue
    {
        public ConductivityValue(double nativeValue, string nativeUnits = ConductivityDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = ConductivityDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}