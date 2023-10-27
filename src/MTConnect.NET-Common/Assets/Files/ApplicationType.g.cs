// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.Files
{
    public enum ApplicationType
    {
        /// <summary>
        /// Generic data.
        /// </summary>
        DATA,
        
        /// <summary>
        /// Computer aided design files or drawings.
        /// </summary>
        DESIGN,
        
        /// <summary>
        /// Documentation regarding a category of file.
        /// </summary>
        DOCUMENTATION,
        
        /// <summary>
        /// User instructions regarding the execution of a task.
        /// </summary>
        INSTRUCTIONS,
        
        /// <summary>
        /// Data related to the history of a machine or process.
        /// </summary>
        LOG,
        
        /// <summary>
        /// Machine instructions to perform a process.
        /// </summary>
        PRODUCTION_PROGRAM
    }
}