// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// The measurement of a sound level or sound pressure level relative to atmospheric pressure.
    /// </summary>
    public class SoundLevelValue : SampleValue
    {
        protected override string MetricUnits => "DECIBEL";
        protected override string InchUnits => "DECIBEL";


        public SoundLevelValue(double degrees)
        {
            Value = degrees;
        }
    }
}
