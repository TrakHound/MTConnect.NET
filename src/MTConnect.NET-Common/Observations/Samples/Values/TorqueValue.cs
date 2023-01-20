// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The measurement of the turning force exerted on an object or by an object.
    /// </summary>
    public class TorqueValue : SampleValue
    {
        public TorqueValue(double nativeValue, string nativeUnits = TorqueDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = TorqueDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}