using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TflRoadStatusCheckerClient.Contract.Models;
using TflRoadStatusCheckerClient.Contract.TflDTOs;

namespace TflRoadStatusCheckerClient.Service
{
    public class TflRoadStatusCheckerAPIService : ITflRoadStatusCheckerAPIService<TflRoadStatusCheckerResponse>
    {
        private readonly TflRoadStatusCredentials _apiCredentials;
        private readonly HttpClient _client;

        public TflRoadStatusCheckerAPIService(HttpClient client, IOptions<TflRoadStatusCredentials> apiCredentials)
        {
            _client = client;
            _apiCredentials = apiCredentials.Value;
        }

        public async Task<TflRoadStatusCheckerResponse> GetRoadStatusAsync(string roadName, CancellationToken token)
        {
            if (!ValidateApiKeys())
            {
                throw new InvalidOperationException("TfL Api keys have not been initialized. Please check the credentials!");
            }
            var tflRoadStatusCheckerResponse = new TflRoadStatusCheckerResponse();
            var requestUrl = $"{roadName}?app_id={_apiCredentials.AppId}&app_key={_apiCredentials.ApiKey}";
            var response = await _client.GetAsync(requestUrl, token);

            var responseString = response.Content.ReadAsStringAsync().Result;

            if (!response.IsSuccessStatusCode && response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                var apiError = JsonConvert.DeserializeObject<ApiError>(responseString);
                tflRoadStatusCheckerResponse.Message = apiError.message;
                tflRoadStatusCheckerResponse.isValidRoad = false;
                return tflRoadStatusCheckerResponse;
            }
            if (!response.IsSuccessStatusCode && response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                tflRoadStatusCheckerResponse.Message = responseString;
                tflRoadStatusCheckerResponse.isValidRoad = false;
                return tflRoadStatusCheckerResponse;
            }
            var roadCorridor = JsonConvert.DeserializeObject<RoadCorridor[]>(responseString);
            if (roadCorridor.Length > 0)
            {
                tflRoadStatusCheckerResponse.DisplayName = roadCorridor[0].displayName;
                tflRoadStatusCheckerResponse.Id = roadCorridor[0].id;
                tflRoadStatusCheckerResponse.StatusSeverity = roadCorridor[0].statusSeverity;
                tflRoadStatusCheckerResponse.StatusSeverityDescription = roadCorridor[0].statusSeverityDescription;
                tflRoadStatusCheckerResponse.isValidRoad = true;
            }
            else
            {
                tflRoadStatusCheckerResponse.isValidRoad = false;
                tflRoadStatusCheckerResponse.Message = "Could not fetch the Road status";
            }
            return tflRoadStatusCheckerResponse;
        }

        private bool ValidateApiKeys()
        {
            return !(string.IsNullOrEmpty(_apiCredentials.AppId) && string.IsNullOrEmpty(_apiCredentials.ApiKey));
        }
    }
}