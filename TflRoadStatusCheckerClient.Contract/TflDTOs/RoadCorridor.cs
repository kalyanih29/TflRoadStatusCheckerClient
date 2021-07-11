using Newtonsoft.Json;

namespace TflRoadStatusCheckerClient.Contract.TflDTOs
{
    public class RoadCorridor
    {
        public string bounds { get; set; }

        public string displayName { get; set; }

        public string envelope { get; set; }

        public string id { get; set; }

        public string statusSeverity { get; set; }

        public string statusSeverityDescription { get; set; }

        [JsonProperty("$type")]
        public string Type { get; set; }

        public string url { get; set; }
    }
}