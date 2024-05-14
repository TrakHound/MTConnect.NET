// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools
{
    public enum LocationType
    {
        /// <summary>
        /// Location with regard to a tool crib.
        /// </summary>
        CRIB,
        
        /// <summary>
        /// Location associated with an end effector.
        /// </summary>
        END_EFFECTOR,
        
        /// <summary>
        /// Location for a tool that is no longer usable and is awaiting removal from a tool magazine or turret.
        /// </summary>
        EXPIRED_POT,
        
        /// <summary>
        /// Number of the pot in the tool handling system.
        /// </summary>
        POT,
        
        /// <summary>
        /// Location for a tool removed from a tool magazine or turret awaiting transfer to a location outside of the piece of equipment.
        /// </summary>
        REMOVAL_POT,
        
        /// <summary>
        /// Location for a tool removed from a spindle or turret and awaiting return to a tool magazine.
        /// </summary>
        RETURN_POT,
        
        /// <summary>
        /// Location associated with a spindle.
        /// </summary>
        SPINDLE,
        
        /// <summary>
        /// Location for a tool awaiting transfer to a tool magazine or turret from outside of the piece of equipment.
        /// </summary>
        STAGING_POT,
        
        /// <summary>
        /// Tool location in a horizontal turning machine.
        /// </summary>
        STATION,
        
        /// <summary>
        /// Location for a tool awaiting transfer from a tool magazine to spindle or a turret.
        /// </summary>
        TRANSFER_POT
    }
}