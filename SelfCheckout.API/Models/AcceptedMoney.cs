using SelfCheckout.Abstraction.Enums;

namespace SelfCheckout.API.Models
{
    public class AcceptedMoney
    {
        public Type Type { get; set; }
        public int Value { get; set; }
    }
}