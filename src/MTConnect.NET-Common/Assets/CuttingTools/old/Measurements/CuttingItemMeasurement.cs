// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    public abstract class CuttingItemMeasurement : Measurement
    {
        public CuttingItemMeasurement() { }

        public CuttingItemMeasurement(Measurement measurement) : base(measurement) { }
    }
}