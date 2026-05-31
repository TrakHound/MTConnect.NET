// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.References;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for a <c>ComponentReference</c> in
    /// the cppagent-compatible shape, pointing at a component by its
    /// <c>id</c> with an optional display name.
    /// </summary>
    public class JsonComponentReference
    {
        /// <summary>
        /// Reference to the <c>id</c> of the target component.
        /// </summary>
        [JsonPropertyName("idRef")]
        public string IdRef { get; set; }

        /// <summary>
        /// Optional display name for the reference.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonComponentReference() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IComponentReference"/>.
        /// </summary>
        public JsonComponentReference(IComponentReference reference)
        {
            if (reference != null)
            {
                IdRef = reference.IdRef;
                Name = reference.Name;
            }
        }


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="IReference"/>.
        /// </summary>
        public virtual IReference ToReference()
        {
            var reference = new ComponentReference();
            reference.IdRef = IdRef;
            reference.Name = Name;
            return reference;
        }
    }
}