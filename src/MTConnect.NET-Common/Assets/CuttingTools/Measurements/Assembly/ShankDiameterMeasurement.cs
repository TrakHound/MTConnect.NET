// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Assets.CuttingTools.Measurements.Assembly
{
    /// <summary>
    /// The dimension of the diameter of a cylindrical portion of a Tool Item or an Adaptive Item that can participate in a connection.
    /// </summary>
    public class ShankDiameterMeasurement : AssemblyMeasurement
    {
        public const string TypeId = "ShankDiameter";
        public const string CodeId = "DMM";


        public ShankDiameterMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIMETER;
        }

        public ShankDiameterMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIMETER;
            CDATA = value;
        }
    }
}
