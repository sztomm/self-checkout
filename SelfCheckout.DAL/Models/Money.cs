using SelfCheckout.Abstraction.Enums;

namespace SelfCheckout.DAL.Models
{
    public class Money : BaseModel
    {
        public Type Type { get; set; }
        public int Value { get; set; }

        public Stock Stock { get; set; } = new();
    }
}
