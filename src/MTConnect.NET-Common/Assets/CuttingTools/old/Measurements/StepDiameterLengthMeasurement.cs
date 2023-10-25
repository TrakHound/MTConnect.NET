// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    /// <summary>
    /// The length of a portion of a stepped tool that is related to a corresponding cutting
    /// diameter measured from the cutting reference point of that cutting diameter to the point on
    /// the next cutting edge at which the diameter starts to change.
    /// </summary>
    public class StepDiameterLengthMeasurement : CuttingItemMeasurement
    {
        public const string TypeId = "StepDiameterLength";
        public const string CodeId = "SDLx";


        public StepDiameterLengthMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIMETER;
        }

        public StepDiameterLengthMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIMETER;
            Value = value;
        }

        public StepDiameterLengthMeasurement(Measurement measurement) : base(measurement)
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIMETER;
        }
    }
}