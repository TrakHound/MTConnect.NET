// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    /// <summary>
    /// Distance between the cutting reference point and the rear backing surface of a turning tool or the axis of a boring bar.
    /// </summary>
    public class FunctionalWidthMeasurement : ToolingMeasurement
    {
        /// <summary>
        /// The MTConnect <c>type</c> value that identifies this Measurement.
        /// </summary>
        public const string TypeId = "FunctionalWidth";

        /// <summary>
        /// The MTConnect <c>code</c> value that identifies this Measurement.
        /// </summary>
        public const string CodeId = "WF";


        /// <summary>
        /// Initializes a new instance with its type and code set to the defaults for this Measurement.
        /// </summary>
        public FunctionalWidthMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            
        }

        /// <summary>
        /// Initializes a new instance with the specified measured value.
        /// </summary>
        /// <param name="value">The measured value.</param>
        public FunctionalWidthMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Value = value;
            
        }

        /// <summary>
        /// Initializes a new instance by copying the specified measurement.
        /// </summary>
        /// <param name="measurement">The measurement to copy.</param>
        public FunctionalWidthMeasurement(IToolingMeasurement measurement) : base(measurement)
        {
            Type = TypeId;
            Code = CodeId;
            
        }
    }
}