// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    /// <summary>
    /// Angle between the driving mechanism locator on a tool item and the main cutting edge.
    /// </summary>
    public class DriveAngleMeasurement : Measurement
    {
        public const string TypeId = "DriveAngle";
        public const string CodeId = "DRVA";


        public DriveAngleMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            
        }

        public DriveAngleMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Value = value;
            
        }

        public DriveAngleMeasurement(IMeasurement measurement) : base(measurement)
        {
            Type = TypeId;
            Code = CodeId;
            
        }
    }
}