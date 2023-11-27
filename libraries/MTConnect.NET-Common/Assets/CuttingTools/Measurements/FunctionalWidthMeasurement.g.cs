// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    /// <summary>
    /// Distance between the cutting reference point and the rear backing surface of a turning tool or the axis of a boring bar.
    /// </summary>
    public class FunctionalWidthMeasurement : Measurement
    {
        public const string TypeId = "FunctionalWidth";
        public const string CodeId = "WF";


        public FunctionalWidthMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            
        }

        public FunctionalWidthMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Value = value;
            
        }

        public FunctionalWidthMeasurement(IMeasurement measurement) : base(measurement)
        {
            Type = TypeId;
            Code = CodeId;
            
        }
    }
}