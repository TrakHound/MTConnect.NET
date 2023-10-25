// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.Files
{
    /// <summary>
    /// Abstract Asset that contains the common properties of the File and FileArchetype types.
    /// </summary>
    public interface IAbstractFile : IAsset
    {
        /// <summary>
        /// Category of application that will use this file.
        /// </summary>
        ApplicationCategory ApplicationCategory { get; }
        
        /// <summary>
        /// Type of application that will use this file.
        /// </summary>
        ApplicationType ApplicationType { get; }
        
        /// <summary>
        /// Remark or interpretation for human interpretation associated with a File or FileArchetype.
        /// </summary>
        MTConnect.Assets.Files.IFileComment FileComment { get; }
        
        /// <summary>
        /// Key-value pair providing additional metadata about a File.
        /// </summary>
        MTConnect.Assets.Files.IFileProperty FileProperty { get; }
        
        /// <summary>
        /// Mime type of the file.
        /// </summary>
        string MediaType { get; }
        
        /// <summary>
        /// Name of the file.
        /// </summary>
        string Name { get; }
    }
}