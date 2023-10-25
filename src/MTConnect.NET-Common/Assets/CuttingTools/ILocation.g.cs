// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools
{
    /// <summary>
    /// Location of the pot or spindle the cutting tool currently resides in.If negativeOverlap or positiveOverlap is provided, the tool reserves additional locations on either side, otherwise if they are not given, no additional locations are required for this tool.If the pot occupies the first or last location, a rollover to the beginning or the end of the indexable values may occur. For example, if there are 64 pots and the tool is in pot 64 with a positiveOverlap of 1, the first pot **MAY** be occupied as well.
    /// </summary>
    public interface ILocation
    {
        /// <summary>
        /// Automatic tool changer associated with a tool.
        /// </summary>
        string AutomaticToolChanger { get; }
        
        /// <summary>
        /// Number of locations at lower index values from this location.
        /// </summary>
        int NegativeOverlap { get; }
        
        /// <summary>
        /// Number of locations at higher index value from this location.
        /// </summary>
        int PositiveOverlap { get; }
        
        /// <summary>
        /// Tool bar associated with a tool.
        /// </summary>
        string ToolBar { get; }
        
        /// <summary>
        /// Tool magazine associated with a tool.
        /// </summary>
        string ToolMagazine { get; }
        
        /// <summary>
        /// Tool rack associated with a tool.
        /// </summary>
        string ToolRack { get; }
        
        /// <summary>
        /// Turret associated with a tool.
        /// </summary>
        string Turret { get; }
        
        /// <summary>
        /// Type of location being identified. When a `POT` or `STATION` type is used, value of Location **MUST** be a numeric value.
        /// </summary>
        MTConnect.Assets.CuttingTools.LocationType Type { get; }
        
        /// <summary>
        /// 
        /// </summary>
        string Value { get; }
    }
}