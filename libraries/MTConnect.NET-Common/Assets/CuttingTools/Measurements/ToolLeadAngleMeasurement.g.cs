// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    /// <summary>
    /// Angle between the tool cutting edge plane and a plane perpendicular to the tool feed plane measured in a plane parallel the xy-plane.
    /// </summary>
    public class ToolLeadAngleMeasurement : ToolingMeasurement
    {
        public const string TypeId = "ToolLeadAngle";
        public const string CodeId = "PSIR";


        public ToolLeadAngleMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            
        }

        public ToolLeadAngleMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Value = value;
            
        }

        public ToolLeadAngleMeasurement(IToolingMeasurement measurement) : base(measurement)
        {
            Type = TypeId;
            Code = CodeId;
            
        }
    }
}