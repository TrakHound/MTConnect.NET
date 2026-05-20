// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.Pallet
{
    /// <summary>
    /// Height of the PhysicalAsset
    /// </summary>
    public class HeightMeasurement : Measurement, IHeightMeasurement
    {
        /// <summary>
        /// The MTConnect <c>type</c> value that identifies this Measurement.
        /// </summary>
        public const string TypeId = "Height";

        /// <summary>
        /// The MTConnect <c>code</c> value that identifies this Measurement.
        /// </summary>
        public const string CodeId = "";


        /// <summary>
        /// Initializes a new instance with its type set to the default for this Measurement.
        /// </summary>
        public HeightMeasurement()
        {
            Type = TypeId;
            
        }

        /// <summary>
        /// Initializes a new instance with the specified measured value.
        /// </summary>
        /// <param name="value">The measured value.</param>
        public HeightMeasurement(double value)
        {
            Type = TypeId;
            Value = value;
            
        }

        /// <summary>
        /// Initializes a new instance by copying the specified measurement.
        /// </summary>
        /// <param name="measurement">The measurement to copy.</param>
        public HeightMeasurement(IMeasurement measurement) : base(measurement)
        {
            Type = TypeId;
            
        }
    }
}