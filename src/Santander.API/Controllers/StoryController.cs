using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Santander.API.Common.Interface;
using Santander.API.Common.Model;
using System.Text.Json;

namespace Santander.API.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class StoryController : ControllerBase
    {
        private readonly ILogger<StoryController> _logger;
        private readonly IStoryProcessor _storyProcessor;

        public StoryController(ILogger<StoryController> logger, IStoryProcessor storyProcessor)
        {
            _logger = logger;
            _storyProcessor = storyProcessor;
        }

        [HttpGet]
        [Route("BestStories")]
        public async Task<IActionResult> BestStories(int topNStories)
        {
            try
            {
                _logger.LogInformation($"BestStories called with param value of {topNStories}");
                var bestStories = await _storyProcessor.GetBestStories(topNStories);
                return Ok(bestStories);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred: {ex}");
                return StatusCode(500);
            }
        }
    }
}
