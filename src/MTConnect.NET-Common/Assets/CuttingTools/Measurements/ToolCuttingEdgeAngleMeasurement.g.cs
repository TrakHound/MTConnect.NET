// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    /// <summary>
    /// Angle between the tool cutting edge plane and the tool feed plane measured in a plane parallel the xy-plane.
    /// </summary>
    public class ToolCuttingEdgeAngleMeasurement : Measurement
    {
        public const string TypeId = "ToolCuttingEdgeAngle";
        public const string CodeId = "KAPR";


        public ToolCuttingEdgeAngleMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            
        }

        public ToolCuttingEdgeAngleMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Value = value;
            
        }

        public ToolCuttingEdgeAngleMeasurement(IMeasurement measurement) : base(measurement)
        {
            Type = TypeId;
            Code = CodeId;
            
        }
    }
}