using Polly;
using Santander.API.Common.Interface;
using Santander.API.Common.Model;
using Santander.API.HackerNews;
using Santander.API.HackerNews.Model;
using Santander.API.HackerNews.Transformers;
using Santander.API.HealthCheck;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks().AddCheck<HealthChecker>(nameof(HealthChecker));

var config = new StoryApiConfig();
builder.Configuration.Bind("StoryApiConfig", config);
builder.Services.AddSingleton(config);

builder.Services
    .AddHttpClient<IStoryApiClient, HackerNewsApiClient>("HackerNewsApiClient")
    .AddPolicyHandler(Policy.BulkheadAsync<HttpResponseMessage>(config.MaxHTTPParallelization, Int32.MaxValue))
    .ConfigureHttpClient((serviceProvider, httpClient) =>
    {
        httpClient.BaseAddress = new Uri(config.BaseUrl);
        httpClient.Timeout = TimeSpan.FromSeconds(config.ConnectionTimeout);
    });

builder.Services.AddSingleton<IStoryApiClient, HackerNewsApiClient>();
builder.Services.AddSingleton<IStoryProcessor, HackerNewsProcessor>();
builder.Services.AddSingleton<IBestStoryTransformer<StoryResponse>, HackerNewsBestStoryTransformer>();

var app = builder.Build();


// Configure the HTTP request pipeline.
app.MapHealthChecks("/health");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
