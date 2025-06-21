// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    /// <summary>
    /// Dimension between two parallel tangents on the outside edge of a flange.
    /// </summary>
    public class FlangeDiameterMaxMeasurement : ToolingMeasurement
    {
        public const string TypeId = "FlangeDiameterMax";
        public const string CodeId = "DF";


        public FlangeDiameterMaxMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            
        }

        public FlangeDiameterMaxMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Value = value;
            
        }

        public FlangeDiameterMaxMeasurement(IToolingMeasurement measurement) : base(measurement)
        {
            Type = TypeId;
            Code = CodeId;
            
        }
    }
}