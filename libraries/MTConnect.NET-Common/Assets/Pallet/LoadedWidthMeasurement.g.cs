// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.Pallet
{
    /// <summary>
    /// Loaded width of the PhysicalAsset
    /// </summary>
    public class LoadedWidthMeasurement : Measurement, ILoadedWidthMeasurement
    {
        public new const string DescriptionText = "Loaded width of the PhysicalAsset";
        public const string TypeId = "LoadedWidth";
        public const string CodeId = "";


        public LoadedWidthMeasurement()
        {
            Type = TypeId;
            
        }

        public LoadedWidthMeasurement(double value)
        {
            Type = TypeId;
            Value = value;
            
        }

        public LoadedWidthMeasurement(IMeasurement measurement) : base(measurement)
        {
            Type = TypeId;
            
        }
    }
}