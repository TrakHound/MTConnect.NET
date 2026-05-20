// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.References;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for a <c>DataItemReference</c> in
    /// the cppagent-compatible shape, pointing at a data item by its
    /// <c>id</c> with an optional display name.
    /// </summary>
    public class JsonDataItemReference
    {
        /// <summary>
        /// Reference to the <c>id</c> of the target data item.
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
        public JsonDataItemReference() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IDataItemReference"/>.
        /// </summary>
        public JsonDataItemReference(IDataItemReference reference)
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
            var reference = new DataItemReference();
            reference.IdRef = IdRef;
            reference.Name = Name;
            return reference;
        }
    }
}