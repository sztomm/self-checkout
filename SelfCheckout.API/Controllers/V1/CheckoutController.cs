using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SelfCheckout.API.Exceptions;
using SelfCheckout.API.Interfaces;
using SelfCheckout.API.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SelfCheckout.API.Controllers.V1
{
    /// <summary>
    /// Endpoints that handle checkouts
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class CheckoutController : ControllerBase
    {
        public ILogger<CheckoutController> Logger { get; }
        public IStockService StockService { get; }
        public ICheckoutService CheckoutService { get; }

        public CheckoutController(ILogger<CheckoutController> logger, IStockService stockService, ICheckoutService checkoutService)
        {
            Logger = logger;
            StockService = stockService;
            CheckoutService = checkoutService;
        }

        /// <summary>
        /// Handle inserted moneys and money back
        /// </summary>
        /// <returns>Returns each money type with the amount of it for money back, otherwise return 404 NotFound or 500 Internal Server Error</returns>
        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<IDictionary<string, int>>> HandleCheckout([FromBody] CheckoutVM checkoutVM)
        {
            try
            {
                Logger.LogInformation($"Handling checkout for {nameof(checkoutVM.Price)} {checkoutVM.Price}");

                (var isValid, var message) = await StockService.ValidateMoney(checkoutVM.Inserted.Keys);
                if (!isValid)
                {
                    return BadRequest(message);
                }

                if (checkoutVM.Inserted.Values.Any(amount => amount < 0))
                {
                    Logger.LogWarning($"Cannot handle checkout with negative amount of bills or coins");
                    return BadRequest($"Cannot handle checkout with negative amount of bills or coins");
                }

                var moneyBack = await CheckoutService.CheckSufficiency(checkoutVM.Inserted, checkoutVM.Price);
                if (moneyBack < 0)
                {
                    return BadRequest($"The inserted amount of money is not enough");
                }

                await StockService.FillMoney(checkoutVM.Inserted);
                var splittedMoneyBack = await CheckoutService.CalculateMoneyBack(moneyBack);

                Logger.LogInformation($"Successfully handled checkout");
                return Ok(splittedMoneyBack);
            }
            catch (ImpossibleMoneyBackException imex)
            {
                Logger.LogWarning(imex.Message);
                return BadRequest(imex.Message);
            }
        }
    }
}
