using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Threading.Tasks;

namespace self_checkout.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthCheckController : ControllerBase
    {
        public ILogger<HealthCheckController> Logger { get; }


        public HealthCheckController(ILogger<HealthCheckController> logger)
        {
            Logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<string>> Get()
        {
            Logger.LogDebug($"Alive and well ^-^ Version: {Assembly.GetEntryAssembly()?.GetName().Version}");
            return Ok($"Alive and well ^-^\nVersion: {Assembly.GetEntryAssembly()?.GetName().Version}");
        }
    }
}