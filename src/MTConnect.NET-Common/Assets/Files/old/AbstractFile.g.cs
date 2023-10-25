// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_45f01b9_1589825445428_505818_673

namespace MTConnect.Assets.Files
{
    /// <summary>
    /// Abstract Asset that contains the common properties of the File and FileArchetype types.
    /// </summary>
    public abstract class AbstractFile : Asset, IAbstractFile
    {
        public new const string DescriptionText = "Abstract Asset that contains the common properties of the File and FileArchetype types.";


        /// <summary>
        /// Category of application that will use this file.
        /// </summary>
        public ApplicationCategory ApplicationCategory { get; set; }
        
        /// <summary>
        /// Type of application that will use this file.
        /// </summary>
        public ApplicationType ApplicationType { get; set; }
        
        /// <summary>
        /// Remark or interpretation for human interpretation associated with a File or FileArchetype.
        /// </summary>
        public MTConnect.Assets.Files.IFileComment FileComment { get; set; }
        
        /// <summary>
        /// Key-value pair providing additional metadata about a File.
        /// </summary>
        public MTConnect.Assets.Files.IFileProperty FileProperty { get; set; }
        
        /// <summary>
        /// Mime type of the file.
        /// </summary>
        public string MediaType { get; set; }
        
        /// <summary>
        /// Name of the file.
        /// </summary>
        public string Name { get; set; }
    }
}