// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.Pallet
{
    /// <summary>
    /// Width of the PhysicalAsset
    /// </summary>
    public class WidthMeasurement : Measurement, IWidthMeasurement
    {
        public new const string DescriptionText = "Width of the PhysicalAsset";
        public const string TypeId = "Width";
        public const string CodeId = "";


        public WidthMeasurement()
        {
            Type = TypeId;
            
        }

        public WidthMeasurement(double value)
        {
            Type = TypeId;
            Value = value;
            
        }

        public WidthMeasurement(IMeasurement measurement) : base(measurement)
        {
            Type = TypeId;
            
        }
    }
}