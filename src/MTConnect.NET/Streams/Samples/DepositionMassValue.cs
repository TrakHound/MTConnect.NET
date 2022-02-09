// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// The mass of the material deposited in an additive manufacturing process.
    /// </summary>
    public class DepositionMassValue : SampleValue
    {
        public DepositionMassValue(double nativeValue, string nativeUnits = Devices.Samples.DepositionMassDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = Devices.Samples.DepositionMassDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
