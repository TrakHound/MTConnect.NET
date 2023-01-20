// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    /// <summary>
    /// The distance measured along the X axis from that point of the item closest to the
    /// workpiece, including the Cutting Item for a Tool Item but excluding a protruding
    /// locking mechanism for an Adaptive Item, to either the front of the flange on a flanged
    /// body or the beginning of the connection interface feature on the machine side for
    /// cylindrical or prismatic shanks.
    /// </summary>
    public class BodyLengthMaxMeasurement : AssemblyMeasurement
    {
        public const string TypeId = "BodyLengthMax";
        public const string CodeId = "LBX";


        public BodyLengthMaxMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIMETER;
        }

        public BodyLengthMaxMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIMETER;
            Value = value;
        }

        public BodyLengthMaxMeasurement(Measurement measurement) : base(measurement)
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIMETER;
        }
    }
}
