// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// The measurement of the electrical potential between two points in an electrical circuit in which the current periodically reverses direction.
    /// </summary>
    public class VoltageACValue : SampleValue
    {
        protected override string MetricUnits => "VOLT";
        protected override string InchUnits => "VOLT";


        public VoltageACValue(double voltage)
        {
            Value = voltage;
        }
    }
}
