using System.Collections.Generic;
using System.Threading.Tasks;

namespace SelfCheckout.API.Interfaces
{
    public interface ICheckoutService
    {
        Task<int> CheckSufficiency(IDictionary<string, int> insertedMoney, int price);
        Task<IDictionary<string, int>> CalculateMoneyBack(int moneyBack);
    }
}
