// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = EAID_AB33F1B2_493B_4f60_9394_4A69B30576F9

namespace MTConnect.Assets.CuttingTools
{
    /// <summary>
    /// Number of times the cutter has been reconditioned.
    /// </summary>
    public class ReconditionCount : IReconditionCount
    {
        public const string DescriptionText = "Number of times the cutter has been reconditioned.";


        /// <summary>
        /// Maximum number of times the tool may be reconditioned.
        /// </summary>
        public int MaximumCount { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public int Value { get; set; }
    }
}