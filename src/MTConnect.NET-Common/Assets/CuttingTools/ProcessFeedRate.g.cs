// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = EAID_07E81F95_CE0D_4404_9384_30E428006C48

namespace MTConnect.Assets.CuttingTools
{
    /// <summary>
    /// Constrained process feed rate for the tool in mm/s.The value **MAY** contain the nominal process target feed rate if available. If ProcessFeedRate is provided, at least one value of maximum, nominal, or minimum **MUST** be specified.
    /// </summary>
    public class ProcessFeedRate : IProcessFeedRate
    {
        public const string DescriptionText = "Constrained process feed rate for the tool in mm/s.The value **MAY** contain the nominal process target feed rate if available. If ProcessFeedRate is provided, at least one value of maximum, nominal, or minimum **MUST** be specified.";


        /// <summary>
        /// Upper bound for the tool’s process target feedrate.
        /// </summary>
        public double? Maximum { get; set; }
        
        /// <summary>
        /// Lower bound for the tool's feedrate.
        /// </summary>
        public double? Minimum { get; set; }
        
        /// <summary>
        /// Nominal feedrate the tool is designed to operate at.
        /// </summary>
        public double? Nominal { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public double? Value { get; set; }
    }
}