// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.Files;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.Files
{
    public class JsonFileProperty
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }


        public JsonFileProperty() { }

        public JsonFileProperty(IFileProperty fileProperty)
        {
            if (fileProperty != null)
            {
                Name = fileProperty.Name;
                Value = fileProperty.Value;
            }
        }


        public IFileProperty ToFileProperty()
        {
            var fileProperty = new FileProperty();
            fileProperty.Name = Name;
            fileProperty.Value = Value;
            return fileProperty;
        }
    }
}