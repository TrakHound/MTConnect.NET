// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Assets.Files
{
    public enum ApplicationType
    {
        /// <summary>
        /// Computer aided design files or drawings.
        /// </summary>
        DESIGN,

        /// <summary>
        /// Generic data.
        /// </summary>
        DATA,

        /// <summary>
        /// Documentation regarding a category of file.
        /// </summary>
        DOCUMENTATION,

        /// <summary>
        /// User instructions regarding the execution of a task.
        /// </summary>
        INSTRUCTIONS,

        /// <summary>
        /// The data related to the history of a machine or process.
        /// </summary>
        LOG,

        /// <summary>
        /// Machine instructions to perform a process.
        /// </summary>
        PRODUCTION_PROGRAM
    }
}
