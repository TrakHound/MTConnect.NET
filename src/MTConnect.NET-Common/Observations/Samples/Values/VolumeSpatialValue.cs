// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The geometric volume of an object or container.
    /// </summary>
    public class VolumeSpatialValue : SampleValue
    {
        public VolumeSpatialValue(double nativeValue, string nativeUnits = VolumeSpatialDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = VolumeSpatialDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}