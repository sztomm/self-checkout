using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SelfCheckout.API.ViewModels
{
    public class CheckoutVM
    {
        public IDictionary<string, int> Inserted { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Price cannot be negative")]
        public int Price { get; set; }
    }
}
