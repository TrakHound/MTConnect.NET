// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    /// <summary>
    /// Dimension of the length of the shank.
    /// </summary>
    public class ShankLengthMeasurement : Measurement
    {
        public const string TypeId = "ShankLength";
        public const string CodeId = "LS";


        public ShankLengthMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            
        }

        public ShankLengthMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Value = value;
            
        }

        public ShankLengthMeasurement(IMeasurement measurement) : base(measurement)
        {
            Type = TypeId;
            Code = CodeId;
            
        }
    }
}