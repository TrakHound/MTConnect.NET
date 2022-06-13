// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    /// <summary>
    /// The diameter of a circle to which all edges of a equilateral and round regular insert are tangential.
    /// </summary>
    public class InscribedCircleDiameterMeasurement : CuttingItemMeasurement
    {
        public const string TypeId = "InscribedCircleDiameter";
        public const string CodeId = "IC";


        public InscribedCircleDiameterMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIMETER;
        }

        public InscribedCircleDiameterMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIMETER;
            Value = value;
        }

        public InscribedCircleDiameterMeasurement(Measurement measurement) : base(measurement)
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIMETER;
        }
    }
}
