// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    /// <summary>
    /// Nominal radius of a rounded corner measured in the X Y-plane.
    /// </summary>
    public class CornerRadiusMeasurement : Measurement
    {
        public const string TypeId = "CornerRadius";
        public const string CodeId = "RE";


        public CornerRadiusMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            
        }

        public CornerRadiusMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Value = value;
            
        }

        public CornerRadiusMeasurement(IMeasurement measurement) : base(measurement)
        {
            Type = TypeId;
            Code = CodeId;
            
        }
    }
}