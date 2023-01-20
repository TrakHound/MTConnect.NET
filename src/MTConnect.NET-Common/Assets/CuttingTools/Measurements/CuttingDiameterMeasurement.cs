// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    /// <summary>
    /// The diameter of a circle on which the defined point Pk located on this Cutting Tool.
    /// The normal of the machined peripheral surface points towards the axis of the Cutting Tool.
    /// </summary>
    public class CuttingDiameterMeasurement : CuttingItemMeasurement
    {
        public const string TypeId = "CuttingDiameter";
        public const string CodeId = "DCx";


        public CuttingDiameterMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIMETER;
        }

        public CuttingDiameterMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIMETER;
            Value = value;
        }

        public CuttingDiameterMeasurement(Measurement measurement) : base(measurement)
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIMETER;
        }
    }
}
