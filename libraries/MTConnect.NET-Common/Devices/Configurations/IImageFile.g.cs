// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Reference to a file containing an image of the Component.
    /// </summary>
    public interface IImageFile
    {
        /// <summary>
        /// URL giving the location of the image file.
        /// </summary>
        string Href { get; }
        
        /// <summary>
        /// Unique identifier of the image file.
        /// </summary>
        string Id { get; }
        
        /// <summary>
        /// Mime type of the image file.
        /// </summary>
        string MediaType { get; }
        
        /// <summary>
        /// Description of the image file.
        /// </summary>
        string Name { get; }
    }
}