// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.Files
{
    public enum ApplicationCategory
    {
        /// <summary>
        /// Files regarding the fully assembled product.
        /// </summary>
        ASSEMBLY,
        
        /// <summary>
        /// Device related files.
        /// </summary>
        DEVICE,
        
        /// <summary>
        /// Files relating to the handling of material.
        /// </summary>
        HANDLING,
        
        /// <summary>
        /// Files related to the quality inspection.
        /// </summary>
        INSPECTION,
        
        /// <summary>
        /// Files relating to equipment maintenance.
        /// </summary>
        MAINTENANCE,
        
        /// <summary>
        /// Files relating to a part.
        /// </summary>
        PART,
        
        /// <summary>
        /// Files related to the manufacturing process.
        /// </summary>
        PROCESS,
        
        /// <summary>
        /// Files related to the setup of a process.
        /// </summary>
        SETUP
    }
}