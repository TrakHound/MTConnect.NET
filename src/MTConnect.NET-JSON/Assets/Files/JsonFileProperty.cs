// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Text.Json.Serialization;

namespace MTConnect.Assets.Files
{
    public class JsonFileProperty
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }


        public JsonFileProperty() { }

        public JsonFileProperty(FileProperty fileProperty)
        {
            if (fileProperty != null)
            {
                Name = fileProperty.Name;
                Value = fileProperty.Value;
            }
        }


        public FileProperty ToFileProperty()
        {
            var fileProperty = new FileProperty();
            fileProperty.Name = Name;
            fileProperty.Value = Value;
            return fileProperty;
        }
    }
}
