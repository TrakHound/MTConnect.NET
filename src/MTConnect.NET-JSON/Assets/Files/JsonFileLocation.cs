// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace MTConnect.Assets.Files
{
    public class JsonFileLocation
    {
        [JsonPropertyName("href")]
        public string Href { get; set; }

        [JsonPropertyName("xLinkType")]
        public string xLinkType { get; set; }


        public JsonFileLocation() { }

        public JsonFileLocation(FileLocation fileLocation)
        {
            if (fileLocation != null)
            {
                Href = fileLocation.Href;
                xLinkType = fileLocation.xLinkType;
            }
        }


        public FileLocation ToFileLocation()
        {
            var fileLocation = new FileLocation();
            fileLocation.Href = Href;
            fileLocation.xLinkType = xLinkType;
            return fileLocation;
        }
    }
}