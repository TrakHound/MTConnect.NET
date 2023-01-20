// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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