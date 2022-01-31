// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Assets.CuttingTools.Measurements.CuttingItem
{
    /// <summary>
    /// The distance from the basal plane of the Tool Item to the cutting point.
    /// </summary>
    public class CuttingHeightMeasurement : CuttingItemMeasurement
    {
        public const string TypeId = "CuttingHeight";
        public const string CodeId = "HF";


        public CuttingHeightMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIMETER;
        }

        public CuttingHeightMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIMETER;
            CDATA = value;
        }
    }
}
