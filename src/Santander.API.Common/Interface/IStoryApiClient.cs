using Santander.API.Common.Model;

namespace Santander.API.Common.Interface
{
    public interface IStoryApiClient
    {
        Task<StoryIdResponse> GetBestStoryIds();
        Task<T> GetStory<T>(int storyId);
    }
}
