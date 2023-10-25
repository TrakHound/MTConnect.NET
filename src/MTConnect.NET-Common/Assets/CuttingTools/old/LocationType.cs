// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools
{
    public enum LocationType
    {
        /// <summary>
        /// The number of the pot in the tool handling system.
        /// </summary>
        POT,

        /// <summary>
        /// The tool location in a horizontal turning machine.
        /// </summary>
        STATION,

        /// <summary>
        /// The location with regard to a tool crib.
        /// </summary>
        CRIB,

        SPINDLE
    }
}