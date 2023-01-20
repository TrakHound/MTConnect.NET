// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The change of pressure per unit time.
    /// </summary>
    public class PressurizationRateValue : SampleValue
    {
        public PressurizationRateValue(double nativeValue, string nativeUnits = PressurizationRateDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = PressurizationRateDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}