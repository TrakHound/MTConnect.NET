// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    /// <summary>
    /// Width of the chamfer.
    /// </summary>
    public class ChamferWidthMeasurement : ToolingMeasurement
    {
        /// <summary>
        /// The MTConnect <c>type</c> value that identifies this Measurement.
        /// </summary>
        public const string TypeId = "ChamferWidth";

        /// <summary>
        /// The MTConnect <c>code</c> value that identifies this Measurement.
        /// </summary>
        public const string CodeId = "CHW";


        /// <summary>
        /// Initializes a new instance with its type and code set to the defaults for this Measurement.
        /// </summary>
        public ChamferWidthMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            
        }

        /// <summary>
        /// Initializes a new instance with the specified measured value.
        /// </summary>
        /// <param name="value">The measured value.</param>
        public ChamferWidthMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Value = value;
            
        }

        /// <summary>
        /// Initializes a new instance by copying the specified measurement.
        /// </summary>
        /// <param name="measurement">The measurement to copy.</param>
        public ChamferWidthMeasurement(IToolingMeasurement measurement) : base(measurement)
        {
            Type = TypeId;
            Code = CodeId;
            
        }
    }
}