// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_45f01b9_1589825359572_789274_577

namespace MTConnect.Assets.Files
{
    /// <summary>
    /// Abstractfile type that provides information about the File instance and its URL.
    /// </summary>
    public class File : AbstractFile, IFile
    {
        public new const string DescriptionText = "Abstractfile type that provides information about the File instance and its URL.";


        /// <summary>
        /// Time the file was created.
        /// </summary>
        public System.DateTime CreationTime { get; set; }
        
        /// <summary>
        /// Reference to the target Device for this File.
        /// </summary>
        public string Destination { get; set; }
        
        /// <summary>
        /// Url reference to the file location.
        /// </summary>
        public MTConnect.Assets.Files.IFileLocation Location { get; set; }
        
        /// <summary>
        /// Time the file was modified.
        /// </summary>
        public System.DateTime ModificationTime { get; set; }
        
        /// <summary>
        /// Public key used to verify the signature.
        /// </summary>
        public string PublicKey { get; set; }
        
        /// <summary>
        /// Secure hash of the file.
        /// </summary>
        public string Signature { get; set; }
        
        /// <summary>
        /// Size of the file in bytes.
        /// </summary>
        public int Size { get; set; }
        
        /// <summary>
        /// State of the file.
        /// </summary>
        public MTConnect.Assets.Files.FileState State { get; set; }
        
        /// <summary>
        /// Version identifier of the file.
        /// </summary>
        public string VersionId { get; set; }
    }
}