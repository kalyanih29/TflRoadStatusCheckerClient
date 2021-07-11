using Newtonsoft.Json;

namespace TflRoadStatusCheckerClient.Contract.Models
{
    [JsonObject("TflBaseUrls")]
    public class TflBaseUrls
    {
        [JsonProperty("RoadStatusChecker")]
        public string RoadStatusChecker { get; set; }
    }
}