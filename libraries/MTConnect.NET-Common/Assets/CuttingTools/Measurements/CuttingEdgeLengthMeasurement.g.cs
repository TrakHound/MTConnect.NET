// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    /// <summary>
    /// Theoretical length of the cutting edge of a cutting item over sharp corners.
    /// </summary>
    public class CuttingEdgeLengthMeasurement : Measurement
    {
        public const string TypeId = "CuttingEdgeLength";
        public const string CodeId = "L";


        public CuttingEdgeLengthMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            
        }

        public CuttingEdgeLengthMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Value = value;
            
        }

        public CuttingEdgeLengthMeasurement(IMeasurement measurement) : base(measurement)
        {
            Type = TypeId;
            Code = CodeId;
            
        }
    }
}