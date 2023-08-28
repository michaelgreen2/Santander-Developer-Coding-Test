using System.Text.Json.Serialization;

namespace Santander.API.Common.Model
{
    public record Story
    {
        [JsonIgnore]
        public int StoryId { get; init; }
        [JsonPropertyName("title")]
        public string Title { get; init; }
        [JsonPropertyName("uri")]
        public string Uri { get; init; }
        [JsonPropertyName("postedBy")]
        public string PostedBy { get; init; }
        [JsonPropertyName("time")]
        public DateTimeOffset Time { get; init; }
        [JsonPropertyName("score")]
        public int Score { get; init; }
        [JsonPropertyName("commentCount")]
        public int CommentCount { get; init; }
    }
}
