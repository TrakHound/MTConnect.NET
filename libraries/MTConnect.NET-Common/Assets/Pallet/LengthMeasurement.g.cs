// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.Pallet
{
    /// <summary>
    /// Length of the PhysicalAsset
    /// </summary>
    public class LengthMeasurement : Measurement, ILengthMeasurement
    {
        public new const string DescriptionText = "Length of the PhysicalAsset";
        public const string TypeId = "Length";
        public const string CodeId = "";


        public LengthMeasurement()
        {
            Type = TypeId;
            
        }

        public LengthMeasurement(double value)
        {
            Type = TypeId;
            Value = value;
            
        }

        public LengthMeasurement(IMeasurement measurement) : base(measurement)
        {
            Type = TypeId;
            
        }
    }
}