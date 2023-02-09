// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.References;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonReference
    {
        [JsonPropertyName("idRef")]
        public string IdRef { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }


        public JsonReference() { }

        public JsonReference(IReference reference)
        {
            if (reference != null)
            {
                IdRef = reference.IdRef;
                Name = reference.Name;
            }
        }


        public virtual IReference ToReference()
        {
            var reference = new Reference();
            reference.IdRef = IdRef;
            reference.Name = Name;
            return reference;
        }
    }
}