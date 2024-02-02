// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.Files
{
    /// <summary>
    /// AbstractFile type that provides information about the File instance and its URL.
    /// </summary>
    public partial interface IFileAsset : IAbstractFileAsset
    {
        /// <summary>
        /// Time the file was created.
        /// </summary>
        System.DateTime CreationTime { get; }
        
        /// <summary>
        /// Reference to the target Device for this File.
        /// </summary>
        System.Collections.Generic.IEnumerable<MTConnect.Assets.Files.IDestination> Destinations { get; }
        
        /// <summary>
        /// URL reference to the file location.
        /// </summary>
        MTConnect.Assets.Files.IFileLocation Location { get; }
        
        /// <summary>
        /// Time the file was modified.
        /// </summary>
        System.DateTime? ModificationTime { get; }
        
        /// <summary>
        /// Public key used to verify the signature.
        /// </summary>
        string PublicKey { get; }
        
        /// <summary>
        /// Secure hash of the file.
        /// </summary>
        string Signature { get; }
        
        /// <summary>
        /// Size of the file in bytes.
        /// </summary>
        int Size { get; }
        
        /// <summary>
        /// State of the file.
        /// </summary>
        MTConnect.Assets.Files.FileState State { get; }
        
        /// <summary>
        /// Version identifier of the file.
        /// </summary>
        string VersionId { get; }
    }
}