// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.Files
{
    public enum FileStateEnum
    {
        /// <summary>
        /// Used for processes other than production or otherwise defined.
        /// </summary>
        Experimental,
        
        /// <summary>
        /// Used for production processes.
        /// </summary>
        Production,
        
        /// <summary>
        /// Content is modified from `PRODUCTION` or `EXPERIMENTAL`.
        /// </summary>
        Revision
    }
}