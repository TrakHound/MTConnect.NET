// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.CuttingTools
{
    /// <summary>
    /// JSON serialization surrogate for the typed container of cutter
    /// status values reported on a CuttingTool, keyed by the singular
    /// cppagent element name <c>Status</c>.
    /// </summary>
    public class JsonCutterStatusCollection
    {
        /// <summary>
        /// The cutter statuses in the container.
        /// </summary>
        [JsonPropertyName("Status")]
        public IEnumerable<JsonCutterStatus> Status { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonCutterStatusCollection() { }

        /// <summary>
        /// Initializes the container from a cutter-status sequence.
        /// </summary>
        public JsonCutterStatusCollection(IEnumerable<CutterStatusType> cutterStatuses)
        {
            if (!cutterStatuses.IsNullOrEmpty())
            {
                var status = new List<JsonCutterStatus>();
                foreach (var cutterStatus in cutterStatuses)
                {
                    status.Add(new JsonCutterStatus(cutterStatus));
                }
                Status = status;
            }
        }


        /// <summary>
        /// Flattens the container back into a uniform
        /// <see cref="CutterStatusType"/> sequence.
        /// </summary>
        public IEnumerable<CutterStatusType> ToCutterStatus()
        {
            if (!Status.IsNullOrEmpty())
            {
                var statuses = new List<CutterStatusType>();

                foreach (var status in Status)
                {
                    statuses.Add(status.ToCutterStatus());
                }

                return statuses;
            }

            return null;
        }
    }
}