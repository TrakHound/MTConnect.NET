// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    /// <summary>
    /// Theoretical sharp point of the cutting tool from which the major functional dimensions are taken.
    /// </summary>
    public class CuttingReferencePointMeasurement : Measurement
    {
        public const string TypeId = "CuttingReferencePoint";
        public const string CodeId = "CRP";


        public CuttingReferencePointMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            
        }

        public CuttingReferencePointMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Value = value;
            
        }

        public CuttingReferencePointMeasurement(IMeasurement measurement) : base(measurement)
        {
            Type = TypeId;
            Code = CodeId;
            
        }
    }
}