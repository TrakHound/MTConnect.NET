// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    /// <summary>
    /// The dimension from the yz-plane to the furthest point of the Tool Item or Adaptive Item measured in the -X direction.
    /// </summary>
    public class ProtrudingLengthMeasurement : AssemblyMeasurement
    {
        public const string TypeId = "ProtrudingLength";
        public const string CodeId = "LPR";


        public ProtrudingLengthMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIMETER;
        }

        public ProtrudingLengthMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIMETER;
            Value = value;
        }

        public ProtrudingLengthMeasurement(Measurement measurement) : base(measurement)
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIMETER;
        }
    }
}
