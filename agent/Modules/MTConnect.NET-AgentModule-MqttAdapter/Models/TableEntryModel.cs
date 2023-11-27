using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect
{
    public class TableEntryModel
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("removed")]
        public bool Removed { get; set; }

        [JsonPropertyName("cells")]
        public Dictionary<string, object> Cells { get; set; }
    }
}
