// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// The measurement of power flowing through or dissipated by an electrical circuit or piece of equipment.
    /// </summary>
    public class WattageValue : SampleValue
    {
        protected override string MetricUnits => "WATT";
        protected override string InchUnits => "WATT";


        public WattageValue(double wattage)
        {
            Value = wattage;
        }
    }
}
