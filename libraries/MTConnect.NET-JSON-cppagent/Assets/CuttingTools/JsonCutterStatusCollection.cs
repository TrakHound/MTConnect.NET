// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.CuttingTools
{
    public class JsonCutterStatusCollection
    {
        [JsonPropertyName("Status")]
        public IEnumerable<JsonCutterStatus> Status { get; set; }


        public JsonCutterStatusCollection() { }

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