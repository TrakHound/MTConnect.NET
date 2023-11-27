// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools
{
    /// <summary>
    /// Constrained process spindle speed for the tool in revolutions/minute.The value **MAY** contain the nominal process target spindle speed if available. If ProcessSpindleSpeed is provided, at least one value of maximum, nominal, or minimum **MUST** be specified.
    /// </summary>
    public interface IProcessSpindleSpeed
    {
        /// <summary>
        /// Upper bound for the tool’s target spindle speed.
        /// </summary>
        double? Maximum { get; }
        
        /// <summary>
        /// Lower bound for the tools spindle speed.
        /// </summary>
        double? Minimum { get; }
        
        /// <summary>
        /// Nominal speed the tool is designed to operate at.
        /// </summary>
        double? Nominal { get; }
        
        /// <summary>
        /// 
        /// </summary>
        double? Value { get; }
    }
}