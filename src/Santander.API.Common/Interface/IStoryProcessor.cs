using Santander.API.Common.Model;

namespace Santander.API.Common.Interface
{
    public interface IStoryProcessor
    {
        Task<IEnumerable<Story>> GetBestStories(int topNStories);
    }
}
