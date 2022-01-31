// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Assets.CuttingTools.Measurements.Assembly
{
    /// <summary>
    /// The dimension of the length of the shank.
    /// </summary>
    public class ShankLengthMeasurement : AssemblyMeasurement
    {
        public const string TypeId = "ShankLength";
        public const string CodeId = "LS";


        public ShankLengthMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIMETER;
        }

        public ShankLengthMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIMETER;
            CDATA = value;
        }
    }
}
