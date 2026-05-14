// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.Pallet
{
    /// <summary>
    /// Swing of the PhysicalAsset
    /// </summary>
    public class SwingMeasurement : Measurement, ISwingMeasurement
    {
        public new const string DescriptionText = "Swing of the PhysicalAsset";
        public const string TypeId = "Swing";
        public const string CodeId = "";


        public SwingMeasurement()
        {
            Type = TypeId;
            
        }

        public SwingMeasurement(double value)
        {
            Type = TypeId;
            Value = value;
            
        }

        public SwingMeasurement(IMeasurement measurement) : base(measurement)
        {
            Type = TypeId;
            
        }
    }
}