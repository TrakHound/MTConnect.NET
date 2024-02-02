// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    /// <summary>
    /// Measure of the length of a wiper edge of a cutting item.
    /// </summary>
    public class WiperEdgeLengthMeasurement : Measurement
    {
        public const string TypeId = "WiperEdgeLength";
        public const string CodeId = "BS";


        public WiperEdgeLengthMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            
        }

        public WiperEdgeLengthMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Value = value;
            
        }

        public WiperEdgeLengthMeasurement(IMeasurement measurement) : base(measurement)
        {
            Type = TypeId;
            Code = CodeId;
            
        }
    }
}