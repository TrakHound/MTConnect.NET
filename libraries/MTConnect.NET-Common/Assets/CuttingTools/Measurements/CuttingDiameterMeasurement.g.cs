// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    /// <summary>
    /// Diameter of a circle on which the defined point Pk located on this cutting tool. The normal of the machined peripheral surface points towards the axis of the cutting tool.
    /// </summary>
    public class CuttingDiameterMeasurement : Measurement
    {
        public const string TypeId = "CuttingDiameter";
        public const string CodeId = "DCx";


        public CuttingDiameterMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            
        }

        public CuttingDiameterMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Value = value;
            
        }

        public CuttingDiameterMeasurement(IMeasurement measurement) : base(measurement)
        {
            Type = TypeId;
            Code = CodeId;
            
        }
    }
}