using Newtonsoft.Json;

namespace SelfCheckout.DAL.Models
{
    public class Stock : BaseModel
    {
        public int Count { get; set; }
        internal int MoneyId { get; set; }  // this is needed to configure the dependency for a O2O relationship

        [JsonIgnore]
        public Money Money { get; set; }
    }
}
