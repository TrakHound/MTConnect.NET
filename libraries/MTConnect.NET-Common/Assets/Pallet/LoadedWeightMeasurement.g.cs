// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.Pallet
{
    /// <summary>
    /// Loaded weight of the PhysicalAsset
    /// </summary>
    public class LoadedWeightMeasurement : Measurement, ILoadedWeightMeasurement
    {
        public new const string DescriptionText = "Loaded weight of the PhysicalAsset";
        public const string TypeId = "LoadedWeight";
        public const string CodeId = "";


        public LoadedWeightMeasurement()
        {
            Type = TypeId;
            
        }

        public LoadedWeightMeasurement(double value)
        {
            Type = TypeId;
            Value = value;
            
        }

        public LoadedWeightMeasurement(IMeasurement measurement) : base(measurement)
        {
            Type = TypeId;
            
        }
    }
}