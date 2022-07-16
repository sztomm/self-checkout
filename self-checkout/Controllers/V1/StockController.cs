using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SelfCheckout.API.Interfaces;
using System.Collections.Generic;
using System.Linq;
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
        public IStockService StockService { get; }

        public StockController(ILogger<StockController> logger, IStockService stockService)
        {
            Logger = logger;
            StockService = stockService;
        }

        /// <summary>
        /// Get all stocked moneys from the database
        /// </summary>
        /// <returns>Returns each money type with the amount of it from the database, otherwise return 404 NotFound or 500 Internal Server Error</returns>
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<IDictionary<string, int>>> GetStock()
        {
            Logger.LogInformation($"Getting stocked moneys");   //TODO: use serilog request logger instead

            var stockedMoneys = await StockService.GetStocks();
            if (!stockedMoneys.Any())
            {
                Logger.LogInformation($"Cannot found any stocked money on the database");
                return NotFound($"Cannot found any stocked money on the database");
            }

            Logger.LogDebug($"Mapping stocked moneys to the response object");
            var mappedStockedMoneys = stockedMoneys.ToDictionary(sm => sm.Value.ToString(), sm => sm.Stock.Count);
            Logger.LogDebug($"Successfully mapped stocked moneys to the response object");

            Logger.LogInformation($"Returing {mappedStockedMoneys.Count()} stocked moneys");
            return Ok(mappedStockedMoneys);
        }
    }
}
