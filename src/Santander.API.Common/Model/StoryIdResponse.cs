namespace Santander.API.Common.Model
{
    public record StoryIdResponse
    {
        public StoryIdResponse(int[]? storyIds)
        {
            if (storyIds != null) StoryIds?.AddRange(storyIds);
        }

        public StoryIdResponse(List<int>? storyIds)
        {
            StoryIds = storyIds;
        }

        public List<int>? StoryIds { get; init; } = new();
    }
}