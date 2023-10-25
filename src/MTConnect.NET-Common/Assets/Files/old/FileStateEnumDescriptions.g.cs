// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.Files
{
    public static class FileStateEnumDescriptions
    {
        /// <summary>
        /// Used for processes other than production or otherwise defined.
        /// </summary>
        public const string Experimental = "Used for processes other than production or otherwise defined.";,
        
        /// <summary>
        /// Used for production processes.
        /// </summary>
        public const string Production = "Used for production processes.";,
        
        /// <summary>
        /// Content is modified from `PRODUCTION` or `EXPERIMENTAL`.
        /// </summary>
        public const string Revision = "Content is modified from `PRODUCTION` or `EXPERIMENTAL`.";
    }
}