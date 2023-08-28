using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;

namespace Santander.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthCheckController : ControllerBase
    {
        private readonly ILogger<HealthCheckController> _logger;
        private readonly HealthCheckService _healthCheckService;

        public HealthCheckController(ILogger<HealthCheckController> logger, HealthCheckService healthCheckService)
        {
            _logger = logger;
            _healthCheckService = healthCheckService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                _logger.LogInformation($"Health check called");
                var report = await _healthCheckService.CheckHealthAsync();
                var json = JsonSerializer.Serialize(report);

                if (report.Status == HealthStatus.Healthy)
                    return Ok(json);

                return NotFound("Service unavailable");
            }
            catch (Exception ex) 
            {
                _logger.LogError($"An error occurred: {ex}");
                return StatusCode(500);
            }
        }
    }
}