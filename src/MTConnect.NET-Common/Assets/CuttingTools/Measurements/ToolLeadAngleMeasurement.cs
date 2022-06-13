// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    /// <summary>
    /// The angle between the tool cutting edge plane and a plane perpendicular to the tool feed plane measured in a plane parallel the xy-plane.
    /// </summary>
    public class ToolLeadAngleMeasurement : CuttingItemMeasurement
    {
        public const string TypeId = "ToolLeadAngle";
        public const string CodeId = "PSIR";


        public ToolLeadAngleMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.DEGREE;
        }

        public ToolLeadAngleMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.DEGREE;
            Value = value;
        }

        public ToolLeadAngleMeasurement(Measurement measurement) : base(measurement)
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.DEGREE;
        }
    }
}
