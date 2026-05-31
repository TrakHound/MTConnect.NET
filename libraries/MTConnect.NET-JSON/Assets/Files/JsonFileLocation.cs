// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.Files;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.Files
{
    /// <summary>
    /// JSON serialization surrogate for a <c>FileLocation</c>, the URL at
    /// which a file asset can be retrieved. Converts to and from the
    /// strongly-typed <see cref="FileLocation"/> model.
    /// </summary>
    public class JsonFileLocation
    {
        /// <summary>
        /// The hyperlink to the file.
        /// </summary>
        [JsonPropertyName("href")]
        public string Href { get; set; }

        /// <summary>
        /// The XLink type of the <see cref="Href"/> reference.
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
        /// Converts this surrogate to a strongly-typed <see cref="IFileLocation"/>.
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