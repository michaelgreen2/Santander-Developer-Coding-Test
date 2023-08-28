

# Introduction 
- **Santander.API**: ASP.net Core WebAPI project. This is the start up project comprising of 2 controllers.
  - **HealthCheck**
		- This provides an endpoint for checking health of the API.
  - **Story**: 
		- This provides an endpoint for retrieving the story details of the n best stories. n is a passed parameter specified by the user (as per provided specification).
	
- **Santander.API.Common**: Contain interfaces and POCO's


- **Santander.API.HackerNews**: Contain the Hacker Rank specific code.
  - **HackerNewsBestStoryTransformer**
     - This converts the Hacker Rank Best Story model into the required format, as returned by the Story API. 
  - **HackerNewsApiClient**
     - This calls the Hacker News API to obtain the Story Ids (ranked by score) and the details of each specific story. 
  - **HackerNewsProcessor**
     - This wraps the API calls to **HackerNewsApiClient** and manages the flow / business rules. Story data is cached in this class (see Assumptions) and throttling of the calls to the  Hacker News API is achieve via the Bulkhead feature of the [Polly Nuget](https://github.com/App-vNext/Polly#bulkhead)

# Usage
The story controller exposes an endpoint called **BestStories** and expects an integer parameter, which specifies the how many top stories to return. The endpoint is a GET and can be called like this:

	https://localhost:7282/Story/BestStories?topNStories=2

The returned JSON for the above request would be as per below:

```json
[
  {
    "title": "OpenTF announces fork of Terraform",
    "uri": "https://opentf.org/announcement",
    "postedBy": "cube2222",
    "time": "2023-08-25T14:56:48+00:00",
    "score": 1679,
    "commentCount": 483
  },
  {
    "title": "Factorio: Space Age",
    "uri": "https://factorio.com/blog/post/fff-373",
    "postedBy": "haunter",
    "time": "2023-08-25T11:50:20+00:00",
    "score": 1423,
    "commentCount": 353
  }
]
```
If the **topNStories** parameter is not a valid integer > 0 then an empty array will be returned:



## Appsettings
Within appsettings there is a StoryApiConfig section comprising of the following:

| Item | Description  |
|--|--|
| BaseUrl | Stores the  base URL of the (Hacker News) Story API (https://hacker-news.firebaseio.com) |
| BestStoriesEndPoint | Hacker News endpoint to obtain best stories (/v0/beststories.json) |
| StoryEndPoint | Hacker News endpoint to obtain each story detail  (/v0/item/{0}.json) |
| ConnectionTimeout | Limit (in seconds) when connecting to Hacker News API  |
| MaxHTTPParallelization | Value indicating how many concurrent requests will be used when calling the Hacker News API (this value is used by the Polly Bulkhead.
| CacheTimeout | Value indicating how many seconds to cache story data for. If the value is omitted or a value  that is 0 or less is specified, then no caching will occur.|
| ThreadCount | Value indicating how many separate threads will be used to obtain the story information. For example if this is set to 10 and a user requests the top 100 stories, then 10 threads will be fired, each thread requesting details of 10 stories each.| |
|--|--|

 

# Assumptions
The following assumptions have been made:
 - The story ids returned from https://hacker-news.firebaseio.com/v0/beststories.json are in descending order of score.
 - The stories' scores do not change change so frequently (perhaps every 30mins), thus caching of the best story ids and story data provides a suitable balance between service efficacy and data accuracy. Cache duration can be amended or be disabled.
 - Timestamps provided in the payload via the https://hacker-news.firebaseio.com/v0/item/123.json end point is UTC.



# Future Enhancements / Changes

 - Add benchmarking to provide a view of how long the API is taking to obtain and return story data.
 - Implement logging to disk (serilog for example)
 - Implement Common error handler
 - Unit tests
 - Possibly implement a distributed cache



# Misc.
Feedback appreciated.
