// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    /// <summary>
    /// Flat length of a chamfer.
    /// </summary>
    public class ChamferFlatLengthMeasurement : Measurement
    {
        public const string TypeId = "ChamferFlatLength";
        public const string CodeId = "BCH";


        public ChamferFlatLengthMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            
        }

        public ChamferFlatLengthMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Value = value;
            
        }

        public ChamferFlatLengthMeasurement(IMeasurement measurement) : base(measurement)
        {
            Type = TypeId;
            Code = CodeId;
            
        }
    }
}