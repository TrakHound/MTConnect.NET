// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.Files
{
    /// <summary>
    /// Abstract Asset that contains the common properties of the File and FileArchetype types.
    /// </summary>
    public partial interface IAbstractFileAsset : IAsset
    {
        /// <summary>
        /// Category of application that will use this file.
        /// </summary>
        MTConnect.Assets.Files.ApplicationCategory ApplicationCategory { get; }
        
        /// <summary>
        /// Type of application that will use this file.
        /// </summary>
        MTConnect.Assets.Files.ApplicationType ApplicationType { get; }
        
        /// <summary>
        /// Remark or interpretation for human interpretation associated with a File or FileArchetype.
        /// </summary>
        System.Collections.Generic.IEnumerable<MTConnect.Assets.Files.IFileComment> FileComments { get; }
        
        /// <summary>
        /// Key-value pair providing additional metadata about a File.
        /// </summary>
        System.Collections.Generic.IEnumerable<MTConnect.Assets.Files.IFileProperty> FileProperties { get; }
        
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