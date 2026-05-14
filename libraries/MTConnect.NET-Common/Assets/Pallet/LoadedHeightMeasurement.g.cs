// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.Pallet
{
    /// <summary>
    /// Loaded height of the PhysicalAsset
    /// </summary>
    public class LoadedHeightMeasurement : Measurement, ILoadedHeightMeasurement
    {
        public new const string DescriptionText = "Loaded height of the PhysicalAsset";
        public const string TypeId = "LoadedHeight";
        public const string CodeId = "";


        public LoadedHeightMeasurement()
        {
            Type = TypeId;
            
        }

        public LoadedHeightMeasurement(double value)
        {
            Type = TypeId;
            Value = value;
            
        }

        public LoadedHeightMeasurement(IMeasurement measurement) : base(measurement)
        {
            Type = TypeId;
            
        }
    }
}