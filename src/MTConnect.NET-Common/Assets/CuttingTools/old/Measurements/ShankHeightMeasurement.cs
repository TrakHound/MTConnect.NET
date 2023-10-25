// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    /// <summary>
    /// The dimension of the height of the shank.
    /// </summary>
    public class ShankHeightMeasurement : AssemblyMeasurement
    {
        public const string TypeId = "ShankHeight";
        public const string CodeId = "H";


        public ShankHeightMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIMETER;
        }

        public ShankHeightMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIMETER;
            Value = value;
        }

        public ShankHeightMeasurement(Measurement measurement) : base(measurement)
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIMETER;
        }
    }
}