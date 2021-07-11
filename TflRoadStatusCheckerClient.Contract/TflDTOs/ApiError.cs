using Newtonsoft.Json;
using System;

namespace TflRoadStatusCheckerClient.Contract.TflDTOs
{
    public class ApiError
    {
        public string exceptionType { get; set; }

        public string httpStatus { get; set; }

        public int httpStatusCode { get; set; }

        public string message { get; set; }

        public string relativeUri { get; set; }

        public DateTime timestampUtc { get; set; }

        [JsonProperty("$type")]
        public string Type { get; set; }
    }
}