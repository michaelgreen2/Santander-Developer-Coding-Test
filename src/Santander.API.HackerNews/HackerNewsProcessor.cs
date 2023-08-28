using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Santander.API.Common.Interface;
using Santander.API.Common.Model;
using Santander.API.HackerNews.Model;
using System.Collections.Concurrent;

namespace Santander.API.HackerNews
{
    public class HackerNewsProcessor : IStoryProcessor
    {
        private readonly ILogger<HackerNewsProcessor> _logger;
        private readonly StoryApiConfig _apiConfig;
        private readonly IStoryApiClient _storyApiClient;
        private readonly IBestStoryTransformer<StoryResponse> _bestStoryTransformer;
        private readonly ConcurrentDictionary<int, Story> _stories = new ConcurrentDictionary<int, Story>();
        private readonly IMemoryCache _storyCache;

        public HackerNewsProcessor(ILogger<HackerNewsProcessor> logger, StoryApiConfig apiConfig,
                                   IStoryApiClient storyApiClient, IBestStoryTransformer<StoryResponse> bestStoryTransformer,
                                   IMemoryCache storyCache)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _apiConfig = apiConfig ?? throw new ArgumentNullException(nameof(apiConfig));
            _storyApiClient = storyApiClient ?? throw new ArgumentNullException(nameof(storyApiClient));
            _bestStoryTransformer = bestStoryTransformer ?? throw new ArgumentNullException(nameof(bestStoryTransformer));
            _storyCache = storyCache ?? throw new ArgumentNullException(nameof(storyCache));
        }

        public async Task<IEnumerable<Story>> GetBestStories(int topNStories)
        {
            _logger.LogInformation($"GetBestStories called with param value of {topNStories}.");
            if (topNStories <= 0) return new List<Story>();

            var storyIds = await GetBestStoryIds(topNStories);
            await ProcessAllStories(storyIds);

            var topBestStories = storyIds
                .Join(_stories,
                    storyId => storyId,
                    story => story.Key,
                    (storyId, story) => story.Value)
                .OrderByDescending(s => s.Score);

            return topBestStories;
        }

        private async Task<List<int>> GetBestStoryIds(int topNStories)
        {
            // If no caching then
            if (!_apiConfig.CacheStories)
            {
                _logger.LogInformation("GetBestStoryIds: Caching disabled so hitting story API.");
                _stories.Clear();
                var response = await _storyApiClient.GetBestStoryIds();
                return response.StoryIds.Take(topNStories).ToList();
            }

            // Using cache instead
            _logger.LogInformation("GetBestStoryIds: Caching is enabled.");
            if (!_storyCache.TryGetValue("StoryIds", out List<int>? storyIds))
            {
                // Not in cache so get and store
                _logger.LogInformation("GetBestStoryIds: StoryIds not present in cache or expired - Hitting story API.");
                var response = await _storyApiClient.GetBestStoryIds();

                _logger.LogInformation($"GetBestStoryIds: Storing {response.StoryIds.Count} StoryIds in cache.");
                _storyCache.Set("StoryIds", response.StoryIds, TimeSpan.FromSeconds(_apiConfig.CacheTimeout));
                _stories.Clear();

                return response.StoryIds.Take(topNStories).ToList();
            }

            // Get from cache
            _logger.LogInformation("GetBestStoryIds: StoryIds found in cache.");
            return storyIds.Take(topNStories).ToList();
        }

        private async Task ProcessAllStories(List<int> storyIds)
        {
            _logger.LogInformation($"ProcessAllStories: ThreadCount is set to {_apiConfig.ThreadCount}.");
            var batchSize = 1;
            if (storyIds.Count >= _apiConfig.ThreadCount)
            {
                batchSize = storyIds.Count / _apiConfig.ThreadCount;
            }

            // Split up ids by the number of threads / batches
            var batchedStoryIds = storyIds
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / batchSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();

            var tasks = batchedStoryIds.Select(b => Task.Run(() => ProcessStoryBatch(b)));
            await Task.WhenAll(tasks.ToArray());
        }

        private async Task ProcessStoryBatch(List<int> storyIds)
        {
            _logger.LogInformation($"ProcessStoryBatch: Thread Id is set to {Thread.CurrentThread.ManagedThreadId}.");
            foreach (var storyId in storyIds)
            {
                if (_stories.TryGetValue(storyId, out _))
                {
                    // If story exists in the dictionary skip
                    continue;
                }

                // Hit the API to get the story details
                var storyResponse = await _storyApiClient.GetStory<StoryResponse>(storyId);
                _stories[storyId] = _bestStoryTransformer.Transform(storyResponse);
            }
        }
    }
}
