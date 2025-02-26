// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    /// <summary>
    /// Angle between the major cutting edge and the same cutting edge rotated by 180 degrees about the tool axis.
    /// </summary>
    public class PointAngleMeasurement : ToolingMeasurement
    {
        public const string TypeId = "PointAngle";
        public const string CodeId = "SIG";


        public PointAngleMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            
        }

        public PointAngleMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Value = value;
            
        }

        public PointAngleMeasurement(IToolingMeasurement measurement) : base(measurement)
        {
            Type = TypeId;
            Code = CodeId;
            
        }
    }
}