// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// The measurement of the amount of deformation per unit length of an object when a load is applied.
    /// </summary>
    public class StrainValue : SampleValue
    {
        public StrainValue(double nativeValue, string nativeUnits = Devices.Samples.StrainDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = Devices.Samples.StrainDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
