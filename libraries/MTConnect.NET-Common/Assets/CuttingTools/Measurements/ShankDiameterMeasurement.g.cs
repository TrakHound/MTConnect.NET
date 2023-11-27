// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    /// <summary>
    /// Dimension of the diameter of a cylindrical portion of a tool item or an adaptive item that can participate in a connection.
    /// </summary>
    public class ShankDiameterMeasurement : Measurement
    {
        public const string TypeId = "ShankDiameter";
        public const string CodeId = "DMM";


        public ShankDiameterMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            
        }

        public ShankDiameterMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Value = value;
            
        }

        public ShankDiameterMeasurement(IMeasurement measurement) : base(measurement)
        {
            Type = TypeId;
            Code = CodeId;
            
        }
    }
}