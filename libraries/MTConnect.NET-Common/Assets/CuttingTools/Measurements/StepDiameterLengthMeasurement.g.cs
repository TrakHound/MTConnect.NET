// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    /// <summary>
    /// Length of a portion of a stepped tool that is related to a corresponding cutting diameter measured from the cutting reference point of that cutting diameter to the point on the next cutting edge at which the diameter starts to change.
    /// </summary>
    public class StepDiameterLengthMeasurement : Measurement
    {
        public const string TypeId = "StepDiameterLength";
        public const string CodeId = "SDLx";


        public StepDiameterLengthMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            
        }

        public StepDiameterLengthMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Value = value;
            
        }

        public StepDiameterLengthMeasurement(IMeasurement measurement) : base(measurement)
        {
            Type = TypeId;
            Code = CodeId;
            
        }
    }
}