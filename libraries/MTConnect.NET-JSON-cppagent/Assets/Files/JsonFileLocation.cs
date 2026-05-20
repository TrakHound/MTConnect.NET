// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.Files;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.Files
{
    /// <summary>
    /// JSON serialization surrogate for the URL of a File asset's
    /// content, modelled as an XLink with an optional XLink type
    /// qualifier.
    /// </summary>
    public class JsonFileLocation
    {
        /// <summary>
        /// The URL of the file content.
        /// </summary>
        [JsonPropertyName("href")]
        public string Href { get; set; }

        /// <summary>
        /// The XLink type qualifier (typically <c>locator</c>).
        /// </summary>
        [JsonPropertyName("xLinkType")]
        public string XLinkType { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonFileLocation() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IFileLocation"/>.
        /// </summary>
        public JsonFileLocation(IFileLocation fileLocation)
        {
            if (fileLocation != null)
            {
                Href = fileLocation.Href;
                XLinkType = fileLocation.XLinkType;
            }
        }


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="IFileLocation"/>.
        /// </summary>
        public IFileLocation ToFileLocation()
        {
            var fileLocation = new FileLocation();
            fileLocation.Href = Href;
            fileLocation.XLinkType = XLinkType;
            return fileLocation;
        }
    }
}