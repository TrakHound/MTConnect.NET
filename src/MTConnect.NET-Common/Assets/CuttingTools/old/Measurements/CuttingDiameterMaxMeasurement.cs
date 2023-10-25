// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    /// <summary>
    /// The maximum diameter of a circle on which the defined point Pk of each of the master
    /// inserts is located on a ToolItem. The normal of the machined peripheral surface
    /// points towards the axis of the Cutting Tool.
    /// </summary>
    public class CuttingDiameterMaxMeasurement : AssemblyMeasurement
    {
        public const string TypeId = "CuttingDiameterMax";
        public const string CodeId = "DC";


        public CuttingDiameterMaxMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIMETER;
        }

        public CuttingDiameterMaxMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIMETER;
            Value = value;
        }

        public CuttingDiameterMaxMeasurement(Measurement measurement) : base(measurement)
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIMETER;
        }
    }
}