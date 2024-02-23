// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = EAID_A012A42B_DBEC_4334_891D_5B45D7A7E340

namespace MTConnect.Assets.CuttingTools
{
    /// <summary>
    /// Location of the pot or spindle the cutting tool currently resides in.positiveOverlap is provided, the tool reserves additional locations on either side, otherwise if they are not given, no additional locations are required for this tool.positiveOverlap of 1, the first pot **MAY** be occupied as well.
    /// </summary>
    public class Location : ILocation
    {
        public const string DescriptionText = "Location of the pot or spindle the cutting tool currently resides in.positiveOverlap is provided, the tool reserves additional locations on either side, otherwise if they are not given, no additional locations are required for this tool.positiveOverlap of 1, the first pot **MAY** be occupied as well.";


        /// <summary>
        /// Automatic tool changer associated with a tool.
        /// </summary>
        public string AutomaticToolChanger { get; set; }
        
        /// <summary>
        /// Number of locations at lower index values from this location.
        /// </summary>
        public int? NegativeOverlap { get; set; }
        
        /// <summary>
        /// Number of locations at higher index value from this location.
        /// </summary>
        public int? PositiveOverlap { get; set; }
        
        /// <summary>
        /// Tool bar associated with a tool.
        /// </summary>
        public string ToolBar { get; set; }
        
        /// <summary>
        /// Tool magazine associated with a tool.
        /// </summary>
        public string ToolMagazine { get; set; }
        
        /// <summary>
        /// Tool rack associated with a tool.
        /// </summary>
        public string ToolRack { get; set; }
        
        /// <summary>
        /// Turret associated with a tool.
        /// </summary>
        public string Turret { get; set; }
        
        /// <summary>
        /// Type of location being identified. value**MUST** be a numeric value.
        /// </summary>
        public MTConnect.Assets.CuttingTools.LocationType Type { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string Value { get; set; }
    }
}