using System.Text.Json.Serialization;

namespace SnakeServerAPI.Assets
{
    public class UserData
    {

        [JsonPropertyName("ipAddress")]
        public string Ip { get; set; }
        [JsonPropertyName("city")]
        public string City { get; set; }
    }
}
