// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.References;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate that partitions a component's
    /// references by kind into typed sibling lists, keyed by element
    /// name (<c>ComponentReference</c>, <c>DataItemReference</c>) per
    /// the cppagent shape. Converts to and from a uniform
    /// <see cref="IReference"/> collection.
    /// </summary>
    public class JsonReferenceContainer
    {
        /// <summary>
        /// References to other components.
        /// </summary>
        [JsonPropertyName("ComponentReference")]
        public List<JsonComponentReference> ComponentReferences { get; set; }

        /// <summary>
        /// References to data items.
        /// </summary>
        [JsonPropertyName("DataItemReference")]
        public List<JsonDataItemReference> DataItemReferences { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonReferenceContainer() { }

        /// <summary>
        /// Initializes the container from a uniform reference
        /// collection, partitioning each reference into the typed list
        /// matching its concrete interface.
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
        /// Flattens the typed reference lists back into a uniform
        /// <see cref="IReference"/> collection.
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