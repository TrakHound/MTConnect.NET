// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    /// <summary>
    /// W1 is used for the insert width when an inscribed circle diameter is not practical.
    /// </summary>
    public class InsertWidthMeasurement : ToolingMeasurement
    {
        public const string TypeId = "InsertWidth";
        public const string CodeId = "W1";


        public InsertWidthMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            
        }

        public InsertWidthMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Value = value;
            
        }

        public InsertWidthMeasurement(IToolingMeasurement measurement) : base(measurement)
        {
            Type = TypeId;
            Code = CodeId;
            
        }
    }
}