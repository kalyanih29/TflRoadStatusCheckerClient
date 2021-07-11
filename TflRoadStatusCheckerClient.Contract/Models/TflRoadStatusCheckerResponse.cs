namespace TflRoadStatusCheckerClient.Contract.Models
{
    public class TflRoadStatusCheckerResponse
    {
        public string DisplayName { get; set; }

        public string Id { get; set; }

        public bool isValidRoad { get; set; }

        public string Message { get; set; }

        public string StatusSeverity { get; set; }

        public string StatusSeverityDescription { get; set; }
    }
}