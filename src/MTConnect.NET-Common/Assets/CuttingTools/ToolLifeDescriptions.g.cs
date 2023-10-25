// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools
{
    public static class ToolLifeDescriptions
    {
        /// <summary>
        /// Tool life measured in minutes. All units for minimum, maximum, and nominal **MUST** be provided in minutes.
        /// </summary>
        public const string MINUTES = "Tool life measured in minutes. All units for minimum, maximum, and nominal **MUST** be provided in minutes.";
        
        /// <summary>
        /// Tool life measured in parts. All units for minimum, maximum, and nominal **MUST** be provided as the number of parts.
        /// </summary>
        public const string PART_COUNT = "Tool life measured in parts. All units for minimum, maximum, and nominal **MUST** be provided as the number of parts.";
        
        /// <summary>
        /// Tool life measured in tool wear. Wear **MUST** be provided in millimeters as an offset to nominal. All units for minimum, maximum, and nominal **MUST** be given as millimeter offsets aswell. The standard will only consider dimensional wear at this time.
        /// </summary>
        public const string WEAR = "Tool life measured in tool wear. Wear **MUST** be provided in millimeters as an offset to nominal. All units for minimum, maximum, and nominal **MUST** be given as millimeter offsets aswell. The standard will only consider dimensional wear at this time.";
    }
}