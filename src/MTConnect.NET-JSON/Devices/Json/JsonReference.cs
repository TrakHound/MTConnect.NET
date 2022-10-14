// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

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
