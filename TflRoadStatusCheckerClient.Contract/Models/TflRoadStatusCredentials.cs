using Newtonsoft.Json;

namespace TflRoadStatusCheckerClient.Contract.Models
{
    [JsonObject("TflRoadStatusCredentials")]
    public class TflRoadStatusCredentials
    {
        [JsonProperty("ApiKey")]
        public string ApiKey { get; set; }

        [JsonProperty("AppId")]
        public string AppId { get; set; }
    }
}