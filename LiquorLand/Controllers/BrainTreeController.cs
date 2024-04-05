using Microsoft.AspNetCore.Mvc;
using Braintree;
using System;
using System.Collections.Generic;
using System.Linq;
using LiquorLand.Models;
using Newtonsoft.Json;

namespace LiquorLand.Controllers
{
    public class BrainTreeController : Controller
    {
        public IBraintreeGate _brain {  get; set; }

        public BrainTreeController(IBraintreeGate brain)
        {
            _brain = brain;
        }
        public IActionResult GenerateToken()
        {
            var gateway =_brain.GetGateway();
            var clientToken = gateway.ClientToken.Generate();
            ViewBag.ClientToken = clientToken;
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult GenerateToken(IFormCollection collections)
        {
            decimal amount = 0;
           var shoppingCartString = HttpContext.Session.GetString("cart");
            ShoppingCart? shoppingCart = null;

            if (shoppingCartString != null)
            {
                shoppingCart = JsonConvert.DeserializeObject<ShoppingCart>(shoppingCartString);
                amount = shoppingCart.GetTotal();
            }
            //ShoppingCart totalFee;
            string nonceFormClient = collections["payment_method_nonce"];
            var request = new TransactionRequest
            {
                Amount = amount,    
                PaymentMethodNonce = nonceFormClient,
                OrderId = "55501",
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true
               }
            };
            var gateway = _brain.GetGateway();
            Result<Transaction> result = gateway.Transaction.Sale(request);

            /*if(result.IsSuccess())
            {
                TempData["Success"] = "Transaction was succssful , Amount Charged: $" + result.Target.Amount;
            }*/
            if(result.Target.ProcessorResponseText == "Approved")
            {
                TempData["Success"] = "Transaction was succssful , Amount Charged: $" + result.Target.Amount;
            }
            return RedirectToAction("GenerateToken");
        }
    }
}
