// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.Files
{
    /// <summary>
    /// Remark or interpretation for human interpretation associated with a File or FileArchetype.
    /// </summary>
    public interface IFileComment
    {
        /// <summary>
        /// Time the comment was made.
        /// </summary>
        System.DateTime Timestamp { get; }
        
        /// <summary>
        /// Text of the comment about the file.
        /// </summary>
        string Value { get; }
    }
}