// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Interfaces
{
    public enum InterfaceEventTypes
    {
        /// <summary>
        /// Service to advance material or feed product to a piece of equipment from a continuous or bulk source.
        /// </summary>
        MATERIAL_FEED,

        /// <summary>
        /// Service to change the type of material or product being loaded or fed to a piece of equipment.
        /// </summary>
        MATERIAL_CHANGE,

        /// <summary>
        /// Service to remove or retract material or product.
        /// </summary>
        MATERIAL_RETRACT,

        /// <summary>
        /// Service to change the part or product associated with a piece of equipment to a different part or product.
        /// </summary>
        PART_CHANGE,

        /// <summary>
        /// Service to load a piece of material or product.
        /// </summary>
        MATERIAL_LOAD,

        /// <summary>
        /// Service to unload a piece of material or product.
        /// </summary>
        MATERIAL_UNLOAD,

        /// <summary>
        /// Service to open a door.
        /// </summary>
        OPEN_DOOR,

        /// <summary>
        /// Service to close a door.
        /// </summary>
        CLOSE_DOOR,

        /// <summary>
        /// Service to open a chuck.
        /// </summary>
        OPEN_CHUCK,

        /// <summary>
        /// Service to close a chuck.
        /// </summary>
        CLOSE_CHUCK
    }
}