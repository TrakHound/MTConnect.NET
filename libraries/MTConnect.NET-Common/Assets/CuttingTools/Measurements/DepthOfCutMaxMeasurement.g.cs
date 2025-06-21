// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    /// <summary>
    /// Maximum engagement of the cutting edge or edges with the workpiece measured perpendicular to the feed motion.
    /// </summary>
    public class DepthOfCutMaxMeasurement : ToolingMeasurement
    {
        public const string TypeId = "DepthOfCutMax";
        public const string CodeId = "APMX";


        public DepthOfCutMaxMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            
        }

        public DepthOfCutMaxMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Value = value;
            
        }

        public DepthOfCutMaxMeasurement(IToolingMeasurement measurement) : base(measurement)
        {
            Type = TypeId;
            Code = CodeId;
            
        }
    }
}