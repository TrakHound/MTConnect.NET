// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Assets.Files
{
    public enum FileState
    {
        /// <summary>
        /// Used for processes other than production or otherwise defined.
        /// </summary>
        EXPERIMENTAL,

        /// <summary>
        /// Used for production processes.
        /// </summary>
        PRODUCTION,

        /// <summary>
        /// The content is modified from PRODUCTION or EXPERIMENTAL.
        /// </summary>
        REVISION
    }
}
