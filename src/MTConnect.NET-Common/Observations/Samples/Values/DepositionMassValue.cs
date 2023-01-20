// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The mass of the material deposited in an additive manufacturing process.
    /// </summary>
    public class DepositionMassValue : SampleValue
    {
        public DepositionMassValue(double nativeValue, string nativeUnits = DepositionMassDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = DepositionMassDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
