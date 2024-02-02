// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = EAID_3B2E47CE_BBF6_4b7a_A0C6_146B2BE8331A

namespace MTConnect.Assets.CuttingTools
{
    /// <summary>
    /// Constrained process spindle speed for the tool in revolutions/minute.The value **MAY** contain the nominal process target spindle speed if available. If ProcessSpindleSpeed is provided, at least one value of maximum, nominal, or minimum **MUST** be specified.
    /// </summary>
    public class ProcessSpindleSpeed : IProcessSpindleSpeed
    {
        public const string DescriptionText = "Constrained process spindle speed for the tool in revolutions/minute.The value **MAY** contain the nominal process target spindle speed if available. If ProcessSpindleSpeed is provided, at least one value of maximum, nominal, or minimum **MUST** be specified.";


        /// <summary>
        /// Upper bound for the tool’s target spindle speed.
        /// </summary>
        public double? Maximum { get; set; }
        
        /// <summary>
        /// Lower bound for the tools spindle speed.
        /// </summary>
        public double? Minimum { get; set; }
        
        /// <summary>
        /// Nominal speed the tool is designed to operate at.
        /// </summary>
        public double? Nominal { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public double? Value { get; set; }
    }
}