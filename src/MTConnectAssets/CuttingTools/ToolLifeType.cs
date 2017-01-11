// Copyright (c) 2017 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.MTConnectAssets.CuttingTools
{
    public enum ToolLifeType
    {
        /// <summary>
        /// The tool life measured in minutes. All units for minimum, maximum, and warningLevel MUST be provided in minutes.
        /// </summary>
        MINUTES,

        /// <summary>
        /// The tool life measured in parts. All units for minimum, maximum, and warningLevel MUST be provided supplied as the number of parts.
        /// </summary>
        PART_COUNT,

        /// <summary>
        /// The tool life measured in tool wear. Wear MUST be provided in millimeters as an offset to nominal. All units for minimum, maximum, and warningLevel MUST be given as millimeter offsets as well. The standard will only consider dimensional wear at this time.
        /// </summary>
        WEAR
    }
}
