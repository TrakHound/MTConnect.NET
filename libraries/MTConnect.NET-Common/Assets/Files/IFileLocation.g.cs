// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.Files
{
    /// <summary>
    /// URL reference to the file location.
    /// </summary>
    public interface IFileLocation
    {
        /// <summary>
        /// URL reference to the file.`href` is of type `xlink:href` from the W3C XLink specification.
        /// </summary>
        string Href { get; }
        
        /// <summary>
        /// Type of href for the xlink href type. **MUST** be `locator` referring to a URL.
        /// </summary>
        string XLinkType { get; }
    }
}