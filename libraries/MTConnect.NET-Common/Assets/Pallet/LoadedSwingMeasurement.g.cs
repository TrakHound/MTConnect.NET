// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.Pallet
{
    /// <summary>
    /// Loaded swing of the PhysicalAsset
    /// </summary>
    public class LoadedSwingMeasurement : Measurement, ILoadedSwingMeasurement
    {
        public const string TypeId = "LoadedSwing";
        public const string CodeId = "";


        public LoadedSwingMeasurement()
        {
            Type = TypeId;
            
        }

        public LoadedSwingMeasurement(double value)
        {
            Type = TypeId;
            Value = value;
            
        }

        public LoadedSwingMeasurement(IMeasurement measurement) : base(measurement)
        {
            Type = TypeId;
            
        }
    }
}