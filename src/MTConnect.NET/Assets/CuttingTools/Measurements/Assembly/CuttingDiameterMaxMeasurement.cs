// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Assets.CuttingTools.Measurements.Assembly
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
            CDATA = value;
        }
    }
}
