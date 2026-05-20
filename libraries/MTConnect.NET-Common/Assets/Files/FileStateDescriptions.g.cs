// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.Files
{
    /// <summary>
    /// Description text for each <see cref="FileState"/> value as defined by the MTConnect Standard.
    /// </summary>
    public static class FileStateDescriptions
    {
        /// <summary>
        /// Used for processes other than production or otherwise defined.
        /// </summary>
        public const string EXPERIMENTAL = "Used for processes other than production or otherwise defined.";
        
        /// <summary>
        /// Used for production processes.
        /// </summary>
        public const string PRODUCTION = "Used for production processes.";
        
        /// <summary>
        /// Content is modified from `PRODUCTION` or `EXPERIMENTAL`.
        /// </summary>
        public const string REVISION = "Content is modified from `PRODUCTION` or `EXPERIMENTAL`.";


        /// <summary>
        /// Returns the MTConnect Standard description text for the specified <see cref="FileState"/> value, or <c>null</c> when none is defined.
        /// </summary>
        public static string Get(FileState value)
        {
            switch (value)
            {
                case FileState.EXPERIMENTAL: return "Used for processes other than production or otherwise defined.";
                case FileState.PRODUCTION: return "Used for production processes.";
                case FileState.REVISION: return "Content is modified from `PRODUCTION` or `EXPERIMENTAL`.";
            }

            return null;
        }
    }
}