using SelfCheckout.API.Models;
using System.Collections.Generic;

namespace SelfCheckout.API.Configurations
{
    public class MoneysConfiguration
    {
        public List<AcceptedMoney> AcceptedMoneys { get; set; } = new();
    }
}
