// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    /// <summary>
    /// Largest length dimension of the cutting tool including the master insert where applicable.
    /// </summary>
    public class OverallToolLengthMeasurement : ToolingMeasurement
    {
        public const string TypeId = "OverallToolLength";
        public const string CodeId = "OAL";


        public OverallToolLengthMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            
        }

        public OverallToolLengthMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Value = value;
            
        }

        public OverallToolLengthMeasurement(IToolingMeasurement measurement) : base(measurement)
        {
            Type = TypeId;
            Code = CodeId;
            
        }
    }
}