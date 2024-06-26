// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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
        /// Content is modified from `PRODUCTION` or `EXPERIMENTAL`.
        /// </summary>
        REVISION
    }
}