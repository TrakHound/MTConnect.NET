// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    /// <summary>
    /// Distance from the basal plane of the tool item to the cutting point.
    /// </summary>
    public class CuttingHeightMeasurement : ToolingMeasurement
    {
        public const string TypeId = "CuttingHeight";
        public const string CodeId = "HF";


        public CuttingHeightMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            
        }

        public CuttingHeightMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Value = value;
            
        }

        public CuttingHeightMeasurement(IToolingMeasurement measurement) : base(measurement)
        {
            Type = TypeId;
            Code = CodeId;
            
        }
    }
}