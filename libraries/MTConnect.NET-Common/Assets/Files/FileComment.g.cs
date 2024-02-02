// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_68e0225_1605276197212_1938_196

namespace MTConnect.Assets.Files
{
    /// <summary>
    /// Remark or interpretation for human interpretation associated with a File or FileArchetype.
    /// </summary>
    public class FileComment : IFileComment
    {
        public const string DescriptionText = "Remark or interpretation for human interpretation associated with a File or FileArchetype.";


        /// <summary>
        /// Time the comment was made.
        /// </summary>
        public System.DateTime Timestamp { get; set; }
        
        /// <summary>
        /// Text of the comment about the file.
        /// </summary>
        public string Value { get; set; }
    }
}