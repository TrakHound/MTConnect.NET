// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    /// <summary>
    /// The distance from the gauge plane or from the end of the shank to the furthest point on
    /// the tool, if a gauge plane does not exist, to the cutting reference point determined by the main function of the tool.
    /// The CuttingTool functional length will be the length of the entire tool, not a single Cutting Item. 
    /// Each CuttingItem can have an independent FunctionalLength represented in its measurements.
    /// </summary>
    public class FunctionalLengthMeasurement : CommonMeasurement
    {
        public const string TypeId = "FunctionalLength";
        public const string CodeId = "LF";
        public new const string DescriptionText = "The distance from the gauge plane or from the end of the shank to the furthest point on the tool, if a gauge plane does not exist, to the cutting reference point determined by the main function of the tool. The CuttingTool functional length will be the length of the entire tool, not a single Cutting Item. Each CuttingItem can have an independent FunctionalLength represented in its measurements.";


        public FunctionalLengthMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIMETER;
        }

        public FunctionalLengthMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIMETER;
            CDATA = value;
        }
    }
}
