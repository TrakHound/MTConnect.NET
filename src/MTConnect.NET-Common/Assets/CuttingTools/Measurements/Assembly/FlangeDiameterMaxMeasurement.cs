// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Assets.CuttingTools.Measurements.Assembly
{
    /// <summary>
    /// The dimension between two parallel tangents on the outside edge of a flange.
    /// </summary>
    public class FlangeDiameterMaxMeasurement : AssemblyMeasurement
    {
        public const string TypeId = "FlangeDiamterMax";
        public const string CodeId = "DF";


        public FlangeDiameterMaxMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIMETER;
        }

        public FlangeDiameterMaxMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIMETER;
            CDATA = value;
        }
    }
}
