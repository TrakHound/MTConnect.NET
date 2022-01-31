// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Assets.CuttingTools.Measurements.CuttingItem
{
    /// <summary>
    /// The nominal radius of a rounded corner measured in the X Y-plane.
    /// </summary>
    public class CornerRadiusMeasurement : CuttingItemMeasurement
    {
        public const string TypeId = "CornerRadius";
        public const string CodeId = "RE";


        public CornerRadiusMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIMETER;
        }

        public CornerRadiusMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIMETER;
            CDATA = value;
        }
    }
}
