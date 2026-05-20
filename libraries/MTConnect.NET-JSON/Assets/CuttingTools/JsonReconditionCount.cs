// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.CuttingTools
{
    /// <summary>
    /// JSON serialization surrogate for a <c>ReconditionCount</c>, recording
    /// how many times a cutting tool has been reconditioned and the maximum
    /// permitted. Converts to and from the strongly-typed
    /// <see cref="ReconditionCount"/> model.
    /// </summary>
    public class JsonReconditionCount
    {
        /// <summary>
        /// The maximum number of times the tool may be reconditioned.
        /// </summary>
        [JsonPropertyName("maximumCount")]
        public int? MaximumCount { get; set; }

        /// <summary>
        /// The number of times the tool has been reconditioned so far.
        /// </summary>
        [JsonPropertyName("value")]
        public int? Value { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonReconditionCount() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IReconditionCount"/>.
        /// </summary>
        public JsonReconditionCount(IReconditionCount reconditionCount)
        {
            if (reconditionCount != null)
            {
                MaximumCount = reconditionCount.MaximumCount;
                Value = reconditionCount.Value;
            }
        }


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="IReconditionCount"/>.
        /// </summary>
        public IReconditionCount ToReconditionCount()
        {
            var reconditionCount = new ReconditionCount();
            reconditionCount.MaximumCount = MaximumCount;
            reconditionCount.Value = Value;
            return reconditionCount;
        }
    }
}