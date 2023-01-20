// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    /// <summary>
    /// The angle between the tool cutting edge plane and the tool feed plane measured in a plane parallel the xy-plane.
    /// </summary>
    public class ToolCuttingEdgeAngleMeasurement : CuttingItemMeasurement
    {
        public const string TypeId = "ToolCuttingEdgeAngle";
        public const string CodeId = "KAPR";


        public ToolCuttingEdgeAngleMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.DEGREE;
        }

        public ToolCuttingEdgeAngleMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.DEGREE;
            Value = value;
        }

        public ToolCuttingEdgeAngleMeasurement(Measurement measurement) : base(measurement)
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.DEGREE;
        }
    }
}
