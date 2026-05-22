// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.Pallet
{
    /// <summary>
    /// Weight of the PhysicalAsset
    /// </summary>
    public class WeightMeasurement : Measurement, IWeightMeasurement
    {
        public const string TypeId = "Weight";
        public const string CodeId = "";


        public WeightMeasurement()
        {
            Type = TypeId;
            
        }

        public WeightMeasurement(double value)
        {
            Type = TypeId;
            Value = value;
            
        }

        public WeightMeasurement(IMeasurement measurement) : base(measurement)
        {
            Type = TypeId;
            
        }
    }
}