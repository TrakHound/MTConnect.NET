// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    /// <summary>
    /// Diameter of a circle to which all edges of a equilateral and round regular insert are tangential.
    /// </summary>
    public class IncribedCircleDiameterMeasurement : Measurement
    {
        public const string TypeId = "IncribedCircleDiameter";
        public const string CodeId = "IC";


        public IncribedCircleDiameterMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            
        }

        public IncribedCircleDiameterMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Value = value;
            
        }

        public IncribedCircleDiameterMeasurement(IMeasurement measurement) : base(measurement)
        {
            Type = TypeId;
            Code = CodeId;
            
        }
    }
}