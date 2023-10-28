// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    /// <summary>
    /// Largest diameter of the body of a tool item.
    /// </summary>
    public class BodyDiameterMaxMeasurement : Measurement
    {
        public const string TypeId = "BodyDiameterMax";
        public const string CodeId = "BDX";


        public BodyDiameterMaxMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            
        }

        public BodyDiameterMaxMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Value = value;
            
        }

        public BodyDiameterMaxMeasurement(IMeasurement measurement) : base(measurement)
        {
            Type = TypeId;
            Code = CodeId;
            
        }
    }
}