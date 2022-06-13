// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    /// <summary>
    /// The largest diameter of the body of a Tool Item.
    /// </summary>
    public class BodyDiameterMaxMeasurement : AssemblyMeasurement
    {
        public const string TypeId = "BodyDiameterMax";
        public const string CodeId = "BDX";


        public BodyDiameterMaxMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIMETER;
        }

        public BodyDiameterMaxMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIMETER;
            Value = value;
        }

        public BodyDiameterMaxMeasurement(Measurement measurement) : base(measurement)
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIMETER;
        }
    }
}
