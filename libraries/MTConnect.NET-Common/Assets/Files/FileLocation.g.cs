// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_68e0225_1605277122154_664309_406

namespace MTConnect.Assets.Files
{
    /// <summary>
    /// URL reference to the file location.
    /// </summary>
    public class FileLocation : IFileLocation
    {
        public const string DescriptionText = "URL reference to the file location.";


        /// <summary>
        /// URL reference to the file.`href` is of type `xlink:href` from the W3C XLink specification.
        /// </summary>
        public string Href { get; set; }
        
        /// <summary>
        /// Type of href for the xlink href type. **MUST** be `locator` referring to a URL.
        /// </summary>
        public string XLinkType { get; set; }
    }
}