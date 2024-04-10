using Braintree;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace LiquorLand.Models
{
    public class BraintreeGate : IBraintreeGate
    {
        public BraintreeConfiguration Options { get; set; }
        private IBraintreeGateway braintreeGateway {  get; set; }

        public BraintreeGate(IOptions<BraintreeConfiguration> options)
        {
            Options = options.Value;
        }
        public IBraintreeGateway CreateGatway()
        {

            return new BraintreeGateway(Options.Environment, Options.MerchantId, Options.PublicKey, Options.PrivateKey);
        }

        public IBraintreeGateway GetGateway()
        {
            if (braintreeGateway == null)
            {
                braintreeGateway = CreateGatway();
            }
            return braintreeGateway;
        }

        
        
    }
}
