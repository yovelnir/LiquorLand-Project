using Braintree; 

namespace LiquorLand.Models
{
    public class BraintreeConfiguration
    {
        public string Environment { get; set; }
        public string MerchantId { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }

        
    }
}
