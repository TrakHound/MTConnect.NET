// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    /// <summary>
    /// Angle of the tool with respect to the workpiece for a given process. The value is application specific.
    /// </summary>
    public class ToolOrientationMeasurement : Measurement
    {
        public const string TypeId = "ToolOrientation";
        public const string CodeId = "N/A";


        public ToolOrientationMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            
        }

        public ToolOrientationMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Value = value;
            
        }

        public ToolOrientationMeasurement(IMeasurement measurement) : base(measurement)
        {
            Type = TypeId;
            Code = CodeId;
            
        }
    }
}