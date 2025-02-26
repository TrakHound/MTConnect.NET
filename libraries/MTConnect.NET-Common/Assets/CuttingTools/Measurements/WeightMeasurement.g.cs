// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    /// <summary>
    /// Total weight of the cutting tool in grams. The force exerted by the mass of the cutting tool.
    /// </summary>
    public class WeightMeasurement : ToolingMeasurement
    {
        public const string TypeId = "Weight";
        public const string CodeId = "WT";


        public WeightMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            
        }

        public WeightMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Value = value;
            
        }

        public WeightMeasurement(IToolingMeasurement measurement) : base(measurement)
        {
            Type = TypeId;
            Code = CodeId;
            
        }
    }
}