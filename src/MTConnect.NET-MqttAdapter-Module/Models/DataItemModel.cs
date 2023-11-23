using System.Text.Json.Serialization;

namespace MTConnect
{
    public class DataItemModel
    {
        [JsonPropertyName("level")]
        public string Level { get; set; }

        [JsonPropertyName("nativeCode")]
        public string NativeCode { get; set; }

        [JsonPropertyName("nativeSeverity")]
        public string NativeSeverity { get; set; }

        [JsonPropertyName("qualifier")]
        public string Qualifier { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
