// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.Files
{
    /// <summary>
    /// Key-value pair providing additional metadata about a File.
    /// </summary>
    public interface IFileProperty
    {
        /// <summary>
        /// Name of the FileProperty.
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// The value of the FileProperty.
        /// </summary>
        string Value { get; }
    }
}