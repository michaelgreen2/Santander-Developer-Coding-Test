namespace Santander.API.Common.Model
{
    public record StoryApiConfig
    {
        private int _threadCount;
        private int _maxHTTPParallelization;

        public string? BaseUrl { get; init; }
        public string? BestStoriesEndPoint { get; init; }
        public string? StoryEndPoint { get; init; }
        public int ConnectionTimeout { get; init; }
        public int CacheTimeout { get; init; }
        public bool CacheStories => CacheTimeout > 0 ? true : false;
        public int ThreadCount
        {
            get => _threadCount;
            set
            {
                _threadCount = value > 0 ? value : 1;
            }
        }
        public int MaxHTTPParallelization
        {
            get => _maxHTTPParallelization;
            set
            {
                _maxHTTPParallelization = value > 0 ? value : 1;
            }
        }
    }
}