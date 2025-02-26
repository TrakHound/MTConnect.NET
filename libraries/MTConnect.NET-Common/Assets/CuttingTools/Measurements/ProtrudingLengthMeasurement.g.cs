// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    /// <summary>
    /// Dimension from the yz-plane to the furthest point of the tool item or adaptive item measured in the -X direction.
    /// </summary>
    public class ProtrudingLengthMeasurement : ToolingMeasurement
    {
        public const string TypeId = "ProtrudingLength";
        public const string CodeId = "LPR";


        public ProtrudingLengthMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            
        }

        public ProtrudingLengthMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Value = value;
            
        }

        public ProtrudingLengthMeasurement(IToolingMeasurement measurement) : base(measurement)
        {
            Type = TypeId;
            Code = CodeId;
            
        }
    }
}