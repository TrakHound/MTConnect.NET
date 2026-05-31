// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.References;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate that groups the references of a component
    /// or composition by referenced kind (component or data item). Each kind is
    /// serialized as a separate JSON array.
    /// </summary>
    public class JsonReferenceContainer
    {
        /// <summary>
        /// References pointing at other components.
        /// </summary>
        [JsonPropertyName("componentReferences")]
        public List<JsonComponentReference> ComponentReferences { get; set; }

        /// <summary>
        /// References pointing at data items.
        /// </summary>
        [JsonPropertyName("dataItemReferences")]
        public List<JsonDataItemReference> DataItemReferences { get; set; }


        /// <summary>
        /// Initializes an empty container.
        /// </summary>
        public JsonReferenceContainer() { }

        /// <summary>
        /// Initializes the container by partitioning
        /// <paramref name="references"/> into component and data item arrays
        /// based on the concrete reference interface of each entry.
        /// </summary>
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

        /// <summary>
        /// Flattens the component and data item arrays back into a single
        /// reference collection.
        /// </summary>
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