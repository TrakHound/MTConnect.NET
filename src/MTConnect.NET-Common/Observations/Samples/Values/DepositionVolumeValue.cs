// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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