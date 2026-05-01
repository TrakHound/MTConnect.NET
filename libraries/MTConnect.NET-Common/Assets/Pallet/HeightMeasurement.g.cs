// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.Pallet
{
    /// <summary>
    /// Height of the PhysicalAsset
    /// </summary>
    public class HeightMeasurement : Measurement, IHeightMeasurement
    {
        public new const string DescriptionText = "Height of the PhysicalAsset";
        public const string TypeId = "Height";
        public const string CodeId = "";


        public HeightMeasurement()
        {
            Type = TypeId;
            
        }

        public HeightMeasurement(double value)
        {
            Type = TypeId;
            Value = value;
            
        }

        public HeightMeasurement(IMeasurement measurement) : base(measurement)
        {
            Type = TypeId;
            
        }
    }
}