// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The measurement of the amount of deformation per unit length of an object when a load is applied.
    /// </summary>
    public class StrainValue : SampleValue
    {
        public StrainValue(double nativeValue, string nativeUnits = StrainDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = StrainDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}