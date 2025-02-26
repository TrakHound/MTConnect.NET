// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    /// <summary>
    /// Dimension of the height of the shank.
    /// </summary>
    public class ShankHeightMeasurement : ToolingMeasurement
    {
        public const string TypeId = "ShankHeight";
        public const string CodeId = "H";


        public ShankHeightMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            
        }

        public ShankHeightMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Value = value;
            
        }

        public ShankHeightMeasurement(IToolingMeasurement measurement) : base(measurement)
        {
            Type = TypeId;
            Code = CodeId;
            
        }
    }
}