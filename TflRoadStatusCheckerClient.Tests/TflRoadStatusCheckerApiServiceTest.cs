using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TflRoadStatusCheckerClient.Contract.Models;
using TflRoadStatusCheckerClient.Contract.TflDTOs;
using TflRoadStatusCheckerClient.Service;

namespace TflRoadStatusCheckerClient.Tests
{
    public class TflRoadStatusCheckerApiServiceTest
    {
        [Test]
        public async Task GetRoadStatus_WithExistingRoadName_ShouldReturnvalid()
        {
            var roadCorridor = GenerateFakeA2RoadCorridor();
            var handlerMock = new Mock<HttpMessageHandler>();
            var credentialsMock = new Mock<IOptions<TflRoadStatusCredentials>>();
            credentialsMock.SetupGet(o => o.Value).Returns(new TflRoadStatusCredentials() { ApiKey = "23fbaaaa38dd407c9ca9a4b68a84cfce", AppId = "5c136fca9f084429ae578a9bf8fe9b43" });
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(roadCorridor), Encoding.UTF8, "application/json")
            };
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(response);
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("https://api.tfl.gov.uk/Road/")
            };
            var statusCheckerAPIService = new TflRoadStatusCheckerAPIService(httpClient, credentialsMock.Object);

            var retrievedRoadStatus = await statusCheckerAPIService.GetRoadStatusAsync("A2", It.IsAny<CancellationToken>());

            Assert.NotNull(retrievedRoadStatus);
            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(1),
               ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
               ItExpr.IsAny<CancellationToken>());

            Assert.IsTrue(retrievedRoadStatus.isValidRoad);
            Assert.AreEqual(retrievedRoadStatus.DisplayName, "A2");
            Assert.AreEqual(retrievedRoadStatus.StatusSeverity, "Good");
            Assert.AreEqual(retrievedRoadStatus.StatusSeverityDescription, "No Exceptional Delays");
        }

        [Test]
        public async Task GetRoadStatus_WithUnexistingRoadName_ShouldReturnInvalid()
        {
            var apiError = GenerateFakeApiErrorForWrongRoadName();
            var handlerMock = new Mock<HttpMessageHandler>();
            var credentialsMock = new Mock<IOptions<TflRoadStatusCredentials>>();
            credentialsMock.SetupGet(o => o.Value).Returns(new TflRoadStatusCredentials() { ApiKey = "23fbaaaa38dd407c9ca9a4b68a84cfce", AppId = "5c136fca9f084429ae578a9bf8fe9b43" });
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound,
                Content = new StringContent(JsonConvert.SerializeObject(apiError), Encoding.UTF8, "application/json")
            };
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(response);
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("https://api.tfl.gov.uk/Road/")
            };
            var statusCheckerAPIService = new TflRoadStatusCheckerAPIService(httpClient, credentialsMock.Object);

            var retrievedRoadStatus = await statusCheckerAPIService.GetRoadStatusAsync(It.IsAny<string>(), It.IsAny<CancellationToken>());

            Assert.NotNull(retrievedRoadStatus);
            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(1),
               ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
               ItExpr.IsAny<CancellationToken>());

            Assert.IsFalse(retrievedRoadStatus.isValidRoad);
        }

        [Test]
        public async Task GetRoadStatus_WithWrongCredentials_ShouldReturnErrorMessage()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            var credentialsMock = new Mock<IOptions<TflRoadStatusCredentials>>();
            credentialsMock.SetupGet(o => o.Value).Returns(new TflRoadStatusCredentials() { ApiKey = "test", AppId = "test" });
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.TooManyRequests,
                Content = new StringContent("Invalid app_key is provided.")
            };
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(response);
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("https://api.tfl.gov.uk/Road/")
            };
            var statusCheckerAPIService = new TflRoadStatusCheckerAPIService(httpClient, credentialsMock.Object);

            var retrievedRoadStatus = await statusCheckerAPIService.GetRoadStatusAsync("A2", It.IsAny<CancellationToken>());

            Assert.NotNull(retrievedRoadStatus);
            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(1),
               ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
               ItExpr.IsAny<CancellationToken>());

            Assert.IsFalse(retrievedRoadStatus.isValidRoad);
            Assert.AreEqual(retrievedRoadStatus.Message, "Invalid app_key is provided.");
        }

        [Test]
        public async Task WhenABadUrlIsProvided_ServiceShouldReturnNull()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            var credentialsMock = new Mock<IOptions<TflRoadStatusCredentials>>();
            credentialsMock.SetupGet(o => o.Value).Returns(new TflRoadStatusCredentials() { ApiKey = "test", AppId = "test" });

            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound,
                Content = new StringContent(JsonConvert.SerializeObject(GenerateFakeApiErrorForWrongUrl()), Encoding.UTF8, "application/json")
            };
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(response);

            var url = new Uri("http://bad.uri");

            var fakeHttpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = url
            };

            var statusCheckerAPIService = new TflRoadStatusCheckerAPIService(fakeHttpClient, credentialsMock.Object);
            var retrievedRoadStatus = await statusCheckerAPIService.GetRoadStatusAsync("A2", It.IsAny<CancellationToken>());

            Assert.IsFalse(retrievedRoadStatus.isValidRoad);
            Assert.AreEqual(retrievedRoadStatus.Message, "Resource not found: http://api:8001");
        }

        private static RoadCorridor[] GenerateFakeA2RoadCorridor()
        {
            return new RoadCorridor[]
            {
                new RoadCorridor(){
                id = "a2",
                url = "/Road/a2",
                bounds = "[[-0.0857,51.44091],[0.17118,51.49438]]",
                envelope = "[[-0.0857,51.44091],[0.17118,51.49438]]",
                Type = "Tfl.Api.Presentation.Entities.RoadCorridor",
                displayName = "A2",
                statusSeverity = "Good",
                statusSeverityDescription = "No Exceptional Delays"
                }
            };
        }

        private static ApiError GenerateFakeApiErrorForWrongRoadName()
        {
            return new ApiError()
            {
                exceptionType = "EntityNotFoundException",
                httpStatus = "NotFound",
                httpStatusCode = (int)HttpStatusCode.NotFound,
                message = "The following road id is not recognised: null",
                relativeUri = "/Road/null",
                timestampUtc = DateTime.UtcNow,
                Type = "Tfl.Api.Presentation.Entities.ApiError"
            };
        }

        private static ApiError GenerateFakeApiErrorForWrongUrl()
        {
            return new ApiError()
            {
                exceptionType = "EntityNotFoundException",
                httpStatus = "NotFound",
                httpStatusCode = (int)HttpStatusCode.NotFound,
                message = "Resource not found: http://api:8001",
                relativeUri = "/Road/A2",
                timestampUtc = DateTime.UtcNow,
                Type = "Tfl.Api.Presentation.Entities.ApiError"
            };
        }
    }
}