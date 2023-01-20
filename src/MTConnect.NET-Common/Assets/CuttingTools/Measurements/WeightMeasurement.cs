// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    /// <summary>
    /// The total weight of the Cutting Tool in grams.The force exerted by the mass of the Cutting Tool.
    /// </summary>
    public class WeightMeasurement : CommonMeasurement
    {
        public const string TypeId = "Weight";
        public const string CodeId = "WT";


        public WeightMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIGRAM;
        }

        public WeightMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIGRAM;
            Value = value;
        }

        public WeightMeasurement(Measurement measurement) : base(measurement)
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIGRAM;
        }
    }
}
