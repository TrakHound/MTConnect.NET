// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    /// <summary>
    /// The distance between the cutting reference point and the rear backing surface of a turning tool or the axis of a boring bar.
    /// </summary>
    public class FunctionalWidthMeasurement : CuttingItemMeasurement
    {
        public const string TypeId = "FunctionalWidth";
        public const string CodeId = "WF";


        public FunctionalWidthMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIMETER;
        }

        public FunctionalWidthMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIMETER;
            Value = value;
        }

        public FunctionalWidthMeasurement(Measurement measurement) : base(measurement)
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIMETER;
        }
    }
}