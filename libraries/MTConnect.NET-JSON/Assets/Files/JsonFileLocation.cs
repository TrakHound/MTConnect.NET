// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.Files;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.Files
{
    public class JsonFileLocation
    {
        [JsonPropertyName("href")]
        public string Href { get; set; }

        [JsonPropertyName("xLinkType")]
        public string XLinkType { get; set; }


        public JsonFileLocation() { }

        public JsonFileLocation(IFileLocation fileLocation)
        {
            if (fileLocation != null)
            {
                Href = fileLocation.Href;
                XLinkType = fileLocation.XLinkType;
            }
        }


        public IFileLocation ToFileLocation()
        {
            var fileLocation = new FileLocation();
            fileLocation.Href = Href;
            fileLocation.XLinkType = XLinkType;
            return fileLocation;
        }
    }
}