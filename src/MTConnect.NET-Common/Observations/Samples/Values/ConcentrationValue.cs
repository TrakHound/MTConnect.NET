// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The measurement of the percentage of one component within a mixture of components
    /// </summary>
    public class ConcentrationValue : SampleValue
    {
        public ConcentrationValue(double nativeValue, string nativeUnits = ConcentrationDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = ConcentrationDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
