using Braintree;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Options;
using Braintree;
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
            /*Environment = System.Environment.GetEnvironmentVariable("BraintreeEnvironment");
            MerchantId = System.Environment.GetEnvironmentVariable("BraintreeMerchantId");
            PublicKey = System.Environment.GetEnvironmentVariable("BraintreePublicKey");
            PrivateKey = System.Environment.GetEnvironmentVariable("BraintreePrivateKey");

            if (MerchantId == null || PublicKey == null || PrivateKey == null)
            {
                Environment = "sandbox";
                MerchantId = "9j4ynyf697k9685t";
                PublicKey = "25sy94dv3rqgg355";
                PrivateKey = "b0d5e1b1fa9dc24c263a3e83a148a7b3";
            }*/

            //return new BraintreeGateway(Environment, MerchantId, PublicKey, PrivateKey);

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
