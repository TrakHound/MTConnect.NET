// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices
{
    /// <summary>
    /// Defines the services or capabilities that the referenced piece of equipment provides relative to this piece of equipment.
    /// </summary>
    public enum Role
    {
        /// <summary>
        /// The associated piece of equipment performs the functions of a System for this piece of equipment. 
        /// In MTConnect, System provides utility type services to support the operation of a piece of equipment
        /// and these services are required for the operation of a piece of equipment.
        /// </summary>
        SYSTEM,

        /// <summary>
        /// The associated piece of equipment performs the functions as an Auxiliary for this piece of equipment. 
        /// In MTConnect, Auxiliary extends the capabilities of a piece of equipment, but is not required for the equipment to function.
        /// </summary>
        AUXILIARY
    }
}
