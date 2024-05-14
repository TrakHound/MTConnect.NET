// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1589825710107_976904_827

namespace MTConnect.Assets.Files
{
    /// <summary>
    /// Key-value pair providing additional metadata about a File.
    /// </summary>
    public class FileProperty : IFileProperty
    {
        public const string DescriptionText = "Key-value pair providing additional metadata about a File.";


        /// <summary>
        /// Name of the FileProperty.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// The value of the FileProperty.
        /// </summary>
        public string Value { get; set; }
    }
}