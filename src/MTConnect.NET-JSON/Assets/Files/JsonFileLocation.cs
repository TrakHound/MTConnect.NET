// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

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
