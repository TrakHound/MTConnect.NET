// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The spatial volume of material deposited in an additive manufacturing process.
    /// </summary>
    public class DepositionVolumeValue : SampleValue
    {
        public DepositionVolumeValue(double nativeValue, string nativeUnits = DepositionVolumeDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = DepositionVolumeDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
