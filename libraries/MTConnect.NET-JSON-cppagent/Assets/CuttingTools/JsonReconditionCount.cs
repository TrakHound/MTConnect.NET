// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.CuttingTools
{
    /// <summary>
    /// JSON serialization surrogate for a CuttingTool
    /// <c>ReconditionCount</c>, tracking how many times the tool has
    /// been reconditioned and an optional limit before it must be
    /// retired.
    /// </summary>
    public class JsonReconditionCount
    {
        /// <summary>
        /// The maximum allowed number of reconditions.
        /// </summary>
        [JsonPropertyName("maximumCount")]
        public int? MaximumCount { get; set; }

        /// <summary>
        /// The current recondition count.
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