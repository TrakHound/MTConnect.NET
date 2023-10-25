// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools
{
    /// <summary>
    /// Cutting tool life as related to the assembly.
    /// </summary>
    public interface IToolLife
    {
        /// <summary>
        /// Indicates if the tool life counts from zero to maximum or maximum to zero.
        /// </summary>
        CountDirectionType CountDirection { get; }
        
        /// <summary>
        /// Initial life of the tool when it is new.
        /// </summary>
        double Initial { get; }
        
        /// <summary>
        /// End of life limit for the tool.
        /// </summary>
        double Limit { get; }
        
        /// <summary>
        /// Type of tool life being accumulated.
        /// </summary>
        MTConnect.Assets.CuttingTools.ToolLife Type { get; }
        
        /// <summary>
        /// Value of ToolLife.
        /// </summary>
        double Value { get; }
        
        /// <summary>
        /// Point at which a tool life warning will be raised.
        /// </summary>
        double Warning { get; }
    }
}