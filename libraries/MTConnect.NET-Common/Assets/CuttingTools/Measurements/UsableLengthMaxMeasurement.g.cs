// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    /// <summary>
    /// Maximum length of a cutting tool that can be used in a particular cutting operation including the non-cutting portions of the tool.
    /// </summary>
    public class UsableLengthMaxMeasurement : Measurement
    {
        public const string TypeId = "UsableLengthMax";
        public const string CodeId = "LUX";


        public UsableLengthMaxMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            
        }

        public UsableLengthMaxMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Value = value;
            
        }

        public UsableLengthMaxMeasurement(IMeasurement measurement) : base(measurement)
        {
            Type = TypeId;
            Code = CodeId;
            
        }
    }
}