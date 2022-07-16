using System;

namespace SelfCheckout.API.Exceptions
{
    [Serializable]
    public class ImpossibleMoneyBackException : ApplicationException
    {
        public ImpossibleMoneyBackException(string message) : base(message)
        {
        }
    }
}
