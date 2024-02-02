// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_68e0225_1677585025728_765757_660

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Reference to a file containing an image of the Component.
    /// </summary>
    public class ImageFile : IImageFile
    {
        public const string DescriptionText = "Reference to a file containing an image of the Component.";


        /// <summary>
        /// URL giving the location of the image file.
        /// </summary>
        public string Href { get; set; }
        
        /// <summary>
        /// Unique identifier of the image file.
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// Mime type of the image file.
        /// </summary>
        public string MediaType { get; set; }
        
        /// <summary>
        /// Description of the image file.
        /// </summary>
        public string Name { get; set; }
    }
}