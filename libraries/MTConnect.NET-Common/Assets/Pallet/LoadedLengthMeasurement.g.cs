// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.Pallet
{
    /// <summary>
    /// Loaded length of the PhysicalAsset
    /// </summary>
    public class LoadedLengthMeasurement : Measurement, ILoadedLengthMeasurement
    {
        public new const string DescriptionText = "Loaded length of the PhysicalAsset";
        public const string TypeId = "LoadedLength";
        public const string CodeId = "";


        public LoadedLengthMeasurement()
        {
            Type = TypeId;
            
        }

        public LoadedLengthMeasurement(double value)
        {
            Type = TypeId;
            Value = value;
            
        }

        public LoadedLengthMeasurement(IMeasurement measurement) : base(measurement)
        {
            Type = TypeId;
            
        }
    }
}