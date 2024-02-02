// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    /// <summary>
    /// Width of the chamfer.
    /// </summary>
    public class ChamferWidthMeasurement : Measurement
    {
        public const string TypeId = "ChamferWidth";
        public const string CodeId = "CHW";


        public ChamferWidthMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            
        }

        public ChamferWidthMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Value = value;
            
        }

        public ChamferWidthMeasurement(IMeasurement measurement) : base(measurement)
        {
            Type = TypeId;
            Code = CodeId;
            
        }
    }
}