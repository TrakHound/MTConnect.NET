// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools
{
    /// <summary>
    /// Constrained process feed rate for the tool in mm/s.The value **MAY** contain the nominal process target feed rate if available. If ProcessFeedRate is provided, at least one value of maximum, nominal, or minimum **MUST** be specified.
    /// </summary>
    public interface IProcessFeedRate
    {
        /// <summary>
        /// Upper bound for the tool’s process target feedrate.
        /// </summary>
        double? Maximum { get; }
        
        /// <summary>
        /// Lower bound for the tool's feedrate.
        /// </summary>
        double? Minimum { get; }
        
        /// <summary>
        /// Nominal feedrate the tool is designed to operate at.
        /// </summary>
        double? Nominal { get; }
        
        /// <summary>
        /// 
        /// </summary>
        double? Value { get; }
    }
}