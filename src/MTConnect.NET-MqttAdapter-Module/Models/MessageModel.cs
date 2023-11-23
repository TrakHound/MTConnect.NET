using System.Text.Json.Serialization;

namespace MTConnect
{
    public class MessageModel
    {
        [JsonPropertyName("nativeCode")]
        public string NativeCode { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
