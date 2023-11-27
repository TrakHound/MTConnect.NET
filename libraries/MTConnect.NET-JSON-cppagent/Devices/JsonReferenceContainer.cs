// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.References;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonReferenceContainer
    {
        [JsonPropertyName("ComponentReference")]
        public List<JsonComponentReference> ComponentReferences { get; set; }

        [JsonPropertyName("DataItemReference")]
        public List<JsonDataItemReference> DataItemReferences { get; set; }


        public JsonReferenceContainer() { }

        public JsonReferenceContainer(IEnumerable<IReference> references)
        {
            if (!references.IsNullOrEmpty())
            {
                var componentReferences = new List<JsonComponentReference>();
                var dataItemReferences = new List<JsonDataItemReference>();
                foreach (var reference in references)
                {
                    var referenceType = reference.GetType();
                    if (typeof(IComponentReference).IsAssignableFrom(referenceType))
                    {
                        componentReferences.Add(new JsonComponentReference((IComponentReference)reference));
                    }
                    else if (typeof(IDataItemReference).IsAssignableFrom(referenceType))
                    {
                        dataItemReferences.Add(new JsonDataItemReference((IDataItemReference)reference));
                    }
                }

                ComponentReferences = componentReferences;
                DataItemReferences = dataItemReferences;
            }
        }

        public IEnumerable<IReference> ToReferences()
        {
            var references = new List<IReference>();

            if (!ComponentReferences.IsNullOrEmpty())
            {
                foreach (var reference in ComponentReferences)
                {
                    references.Add(reference.ToReference());
                }
            }

            if (!DataItemReferences.IsNullOrEmpty())
            {
                foreach (var reference in DataItemReferences)
                {
                    references.Add(reference.ToReference());
                }
            }

            return references;
        }
    }
}