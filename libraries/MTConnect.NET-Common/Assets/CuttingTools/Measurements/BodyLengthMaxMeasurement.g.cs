// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    /// <summary>
    /// Distance measured along the X axis from that point of the item closest to the workpiece, including the cutting item for a tool item but excluding a protruding locking mechanism for an adaptive item, to either the front of the flange on a flanged body or the beginning of the connection interface feature on the machine side for cylindrical or prismatic shanks.
    /// </summary>
    public class BodyLengthMaxMeasurement : Measurement
    {
        public const string TypeId = "BodyLengthMax";
        public const string CodeId = "LBX";


        public BodyLengthMaxMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            
        }

        public BodyLengthMaxMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Value = value;
            
        }

        public BodyLengthMaxMeasurement(IMeasurement measurement) : base(measurement)
        {
            Type = TypeId;
            Code = CodeId;
            
        }
    }
}