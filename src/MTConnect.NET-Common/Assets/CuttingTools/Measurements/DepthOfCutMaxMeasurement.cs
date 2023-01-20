// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    /// <summary>
    /// The maximum engagement of the cutting edge or edges with the workpiece measured
    /// perpendicular to the feed motion.
    /// </summary>
    public class DepthOfCutMaxMeasurement : AssemblyMeasurement
    {
        public const string TypeId = "DepthOfCutMax";
        public const string CodeId = "APMX";


        public DepthOfCutMaxMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIMETER;
        }

        public DepthOfCutMaxMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIMETER;
            Value = value;
        }

        public DepthOfCutMaxMeasurement(Measurement measurement) : base(measurement)
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIMETER;
        }
    }
}
