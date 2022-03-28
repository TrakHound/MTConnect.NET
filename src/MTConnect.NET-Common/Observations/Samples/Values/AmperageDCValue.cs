// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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
