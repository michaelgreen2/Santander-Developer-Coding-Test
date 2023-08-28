using Santander.API.Common.Interface;
using Santander.API.Common.Model;
using Santander.API.HackerNews.Model;

namespace Santander.API.HackerNews.Transformers
{
    public class HackerNewsBestStoryTransformer : IBestStoryTransformer<StoryResponse>
    {
        public Story Transform(StoryResponse storyResponse)
        {
            return new Story()
            {
                StoryId = storyResponse.Id,
                Title = storyResponse.Title,
                Uri = storyResponse.Url,
                PostedBy = storyResponse.By,
                Time = new DateTimeOffset(DateTimeOffset.FromUnixTimeSeconds(storyResponse.Time).DateTime, new TimeSpan(0, 0, 0)),
                Score = storyResponse.Score,
                CommentCount = storyResponse.Descendants,
            };
        }
    }
}
