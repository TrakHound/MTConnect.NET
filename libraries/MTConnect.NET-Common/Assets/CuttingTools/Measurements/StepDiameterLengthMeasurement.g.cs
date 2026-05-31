// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    /// <summary>
    /// Length of a portion of a stepped tool that is related to a corresponding cutting diameter measured from the cutting reference point of that cutting diameter to the point on the next cutting edge at which the diameter starts to change.
    /// </summary>
    public class StepDiameterLengthMeasurement : ToolingMeasurement
    {
        /// <summary>
        /// The MTConnect <c>type</c> value that identifies this Measurement.
        /// </summary>
        public const string TypeId = "StepDiameterLength";

        /// <summary>
        /// The MTConnect <c>code</c> value that identifies this Measurement.
        /// </summary>
        public const string CodeId = "SDLx";


        /// <summary>
        /// Initializes a new instance with its type and code set to the defaults for this Measurement.
        /// </summary>
        public StepDiameterLengthMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            
        }

        /// <summary>
        /// Initializes a new instance with the specified measured value.
        /// </summary>
        /// <param name="value">The measured value.</param>
        public StepDiameterLengthMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Value = value;
            
        }

        /// <summary>
        /// Initializes a new instance by copying the specified measurement.
        /// </summary>
        /// <param name="measurement">The measurement to copy.</param>
        public StepDiameterLengthMeasurement(IToolingMeasurement measurement) : base(measurement)
        {
            Type = TypeId;
            Code = CodeId;
            
        }
    }
}