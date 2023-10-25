// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    /// <summary>
    /// Maximum length of a Cutting Tool that can be used in a particular cutting operation
    /// including the non-cutting portions of the tool.
    /// </summary>
    public class UsableLengthMaxMeasurement : AssemblyMeasurement
    {
        public const string TypeId = "UsableLengthMax";
        public const string CodeId = "LUX";


        public UsableLengthMaxMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIMETER;
        }

        public UsableLengthMaxMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIMETER;
            Value = value;
        }

        public UsableLengthMaxMeasurement(Measurement measurement) : base(measurement)
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIMETER;
        }
    }
}