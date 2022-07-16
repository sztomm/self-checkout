using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SelfCheckout.API.Exceptions;
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

        /// <summary>
        /// Validates the inserted money and fills the stocked money with the inserted ones in the database
        /// </summary>
        /// <returns>Returns each money type with the amount of it from the database, otherwise return 404 NotFound or 500 Internal Server Error</returns>
        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<IDictionary<string, int>>> HandleMoneys([FromBody] IDictionary<string ,int> handleMoneys)
        {
            try
            {
                Logger.LogInformation($"Filling stocked moneys");   //TODO: use serilog request logger instead

                (var isValid, var message) = await StockService.ValidateMoney(handleMoneys.Keys);
                if (!isValid)
                {
                    return BadRequest(message);
                }

                await StockService.FillMoney(handleMoneys);
                var stockedMoneys = await StockService.GetStocks();

                Logger.LogDebug($"Mapping stocked moneys to the response object");
                var mappedStockedMoneys = stockedMoneys.ToDictionary(sm => sm.Value.ToString(), sm => sm.Stock.Count);
                Logger.LogDebug($"Successfully mapped stocked moneys to the response object");

                Logger.LogInformation($"Returing {mappedStockedMoneys.Count()} stocked moneys");
                return Ok(mappedStockedMoneys);
            }
            catch (NegativeMoneyStockException nmsex)
            {
                Logger.LogError(nmsex.Message);
                return BadRequest(nmsex.Message);
            }
        }
    }
}
