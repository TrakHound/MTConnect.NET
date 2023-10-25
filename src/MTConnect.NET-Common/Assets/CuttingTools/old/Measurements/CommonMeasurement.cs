// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    public abstract class CommonMeasurement : Measurement
    {
        public CommonMeasurement() { }

        public CommonMeasurement(Measurement measurement) : base(measurement) { }
    }
}