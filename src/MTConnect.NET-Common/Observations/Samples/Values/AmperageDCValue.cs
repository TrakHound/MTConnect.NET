// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The measurement of an electric current flowing in one direction only.
    /// </summary>
    public class AmperageDCValue : SampleValue
    {
        public AmperageDCValue(double nativeValue, string nativeUnits = AmperageDCDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = AmperageDCDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}