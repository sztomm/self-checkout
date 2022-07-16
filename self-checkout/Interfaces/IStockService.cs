using SelfCheckout.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SelfCheckout.API.Interfaces
{
    public interface IStockService
    {
        Task<IList<Money>> GetStocks();
    }
}