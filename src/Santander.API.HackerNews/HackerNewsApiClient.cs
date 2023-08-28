using Microsoft.Extensions.Logging;
using Polly;
using Polly.RateLimit;
using Santander.API.Common.Interface;
using Santander.API.Common.Model;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Santander.API.HackerNews
{
    public class HackerNewsApiClient : IStoryApiClient
    {
        private readonly ILogger<HackerNewsApiClient> _logger;
        private readonly StoryApiConfig _apiConfig;
        private readonly IHttpClientFactory _clientFactory;

        public HackerNewsApiClient(ILogger<HackerNewsApiClient> logger, StoryApiConfig apiConfig, IHttpClientFactory clientFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _apiConfig = apiConfig ?? throw new ArgumentNullException(nameof(apiConfig));
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
        }

        public async Task<StoryIdResponse> GetBestStoryIds()
        {
            _logger.LogInformation("GetBestStoryIds called.");
            _logger.LogInformation($"Calling end point {_apiConfig.BestStoriesEndPoint}");
            using var request = new HttpRequestMessage(HttpMethod.Get, _apiConfig.BestStoriesEndPoint);
            using var client = _clientFactory.CreateClient("HackerNewsApiClient");
            using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            var stringResponse = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException(stringResponse, null, response.StatusCode);

            var storyIds = JsonSerializer.Deserialize<int[]>(stringResponse);
            _logger.LogInformation($"Story Ids obtained : {storyIds.Count()}");
            return new StoryIdResponse(storyIds);

        }

        public async Task<StoryResponse> GetStory<StoryResponse>(int storyId)
        {
            _logger.LogInformation($"GetStory called with param value of {storyId}.");

            var url = string.Format(_apiConfig.StoryEndPoint, storyId);
            _logger.LogInformation($"Calling end point {url}");
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            using var client = _clientFactory.CreateClient("HackerNewsApiClient");
            using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            var stringResponse = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException(stringResponse, null, response.StatusCode);

            var storyResponse = JsonSerializer.Deserialize<StoryResponse>(stringResponse);
            return storyResponse;
        }
    }
}