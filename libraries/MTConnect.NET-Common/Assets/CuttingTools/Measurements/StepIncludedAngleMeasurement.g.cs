// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    /// <summary>
    /// Angle between a major edge on a step of a stepped tool and the same cutting edge rotated 180 degrees about its tool axis.
    /// </summary>
    public class StepIncludedAngleMeasurement : Measurement
    {
        public const string TypeId = "StepIncludedAngle";
        public const string CodeId = "STAx";


        public StepIncludedAngleMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            
        }

        public StepIncludedAngleMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Value = value;
            
        }

        public StepIncludedAngleMeasurement(IMeasurement measurement) : base(measurement)
        {
            Type = TypeId;
            Code = CodeId;
            
        }
    }
}