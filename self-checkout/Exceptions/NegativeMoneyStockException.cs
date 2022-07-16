using System;

namespace SelfCheckout.API.Exceptions
{
    [Serializable]
    public class NegativeMoneyStockException : ApplicationException
    {
        public NegativeMoneyStockException(string message) : base(message)
        {
        }
    }
}
