// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    /// <summary>
    /// The theoretical sharp point of the Cutting Tool from which the major functional dimensions are taken.
    /// </summary>
    public class CuttingReferencePointMeasurement : CuttingItemMeasurement
    {
        public const string TypeId = "CuttingReferencePoint";
        public const string CodeId = "CRP";


        public CuttingReferencePointMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIMETER;
        }

        public CuttingReferencePointMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIMETER;
            Value = value;
        }

        public CuttingReferencePointMeasurement(Measurement measurement) : base(measurement)
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIMETER;
        }
    }
}
