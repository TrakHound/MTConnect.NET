// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    /// <summary>
    /// Maximum diameter of a circle on which the defined point Pk of each of the master inserts is located on a tool item. The normal of the machined peripheral surface points towards the axis of the cutting tool.
    /// </summary>
    public class CuttingDiameterMaxMeasurement : ToolingMeasurement
    {
        public const string TypeId = "CuttingDiameterMax";
        public const string CodeId = "DC";


        public CuttingDiameterMaxMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            
        }

        public CuttingDiameterMaxMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Value = value;
            
        }

        public CuttingDiameterMaxMeasurement(IToolingMeasurement measurement) : base(measurement)
        {
            Type = TypeId;
            Code = CodeId;
            
        }
    }
}