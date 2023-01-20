// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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