using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SelfCheckout.API.Controllers.V1
{
    /// <summary>
    /// Endpoints that handle stocks
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class StockController : ControllerBase
    {
        public ILogger<StockController> Logger { get; }

        public StockController(ILogger<StockController> logger)
        {
            Logger = logger;
        }

        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<IDictionary<string, int>>> GetStock()
        {
            return Ok();
        }
    }
}
