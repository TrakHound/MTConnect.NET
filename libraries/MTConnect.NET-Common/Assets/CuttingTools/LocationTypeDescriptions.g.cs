// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools
{
    public static class LocationTypeDescriptions
    {
        /// <summary>
        /// Location with regard to a tool crib.
        /// </summary>
        public const string CRIB = "Location with regard to a tool crib.";
        
        /// <summary>
        /// Location associated with an end effector.
        /// </summary>
        public const string END_EFFECTOR = "Location associated with an end effector.";
        
        /// <summary>
        /// Location for a tool that is no longer usable and is awaiting removal from a tool magazine or turret.
        /// </summary>
        public const string EXPIRED_POT = "Location for a tool that is no longer usable and is awaiting removal from a tool magazine or turret.";
        
        /// <summary>
        /// Number of the pot in the tool handling system.
        /// </summary>
        public const string POT = "Number of the pot in the tool handling system.";
        
        /// <summary>
        /// Location for a tool removed from a tool magazine or turret awaiting transfer to a location outside of the piece of equipment.
        /// </summary>
        public const string REMOVAL_POT = "Location for a tool removed from a tool magazine or turret awaiting transfer to a location outside of the piece of equipment.";
        
        /// <summary>
        /// Location for a tool removed from a spindle or turret and awaiting return to a tool magazine.
        /// </summary>
        public const string RETURN_POT = "Location for a tool removed from a spindle or turret and awaiting return to a tool magazine.";
        
        /// <summary>
        /// Location associated with a spindle.
        /// </summary>
        public const string SPINDLE = "Location associated with a spindle.";
        
        /// <summary>
        /// Location for a tool awaiting transfer to a tool magazine or turret from outside of the piece of equipment.
        /// </summary>
        public const string STAGING_POT = "Location for a tool awaiting transfer to a tool magazine or turret from outside of the piece of equipment.";
        
        /// <summary>
        /// Tool location in a horizontal turning machine.
        /// </summary>
        public const string STATION = "Tool location in a horizontal turning machine.";
        
        /// <summary>
        /// Location for a tool awaiting transfer from a tool magazine to spindle or a turret.
        /// </summary>
        public const string TRANSFER_POT = "Location for a tool awaiting transfer from a tool magazine to spindle or a turret.";


        public static string Get(LocationType value)
        {
            switch (value)
            {
                case LocationType.CRIB: return "Location with regard to a tool crib.";
                case LocationType.END_EFFECTOR: return "Location associated with an end effector.";
                case LocationType.EXPIRED_POT: return "Location for a tool that is no longer usable and is awaiting removal from a tool magazine or turret.";
                case LocationType.POT: return "Number of the pot in the tool handling system.";
                case LocationType.REMOVAL_POT: return "Location for a tool removed from a tool magazine or turret awaiting transfer to a location outside of the piece of equipment.";
                case LocationType.RETURN_POT: return "Location for a tool removed from a spindle or turret and awaiting return to a tool magazine.";
                case LocationType.SPINDLE: return "Location associated with a spindle.";
                case LocationType.STAGING_POT: return "Location for a tool awaiting transfer to a tool magazine or turret from outside of the piece of equipment.";
                case LocationType.STATION: return "Tool location in a horizontal turning machine.";
                case LocationType.TRANSFER_POT: return "Location for a tool awaiting transfer from a tool magazine to spindle or a turret.";
            }

            return null;
        }
    }
}