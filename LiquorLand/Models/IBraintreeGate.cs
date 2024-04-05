using Braintree;
using System;
using System.Collections.Generic;
using System.Linq;


namespace LiquorLand.Models
{
    public interface IBraintreeGate
    {
        IBraintreeGateway CreateGatway();
        IBraintreeGateway GetGateway();
    }
}
