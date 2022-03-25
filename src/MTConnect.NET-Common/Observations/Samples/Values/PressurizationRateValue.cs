// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The change of pressure per unit time.
    /// </summary>
    public class PressurizationRateValue : SampleValue
    {
        public PressurizationRateValue(double nativeValue, string nativeUnits = Devices.Samples.PressurizationRateDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = Devices.Samples.PressurizationRateDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
