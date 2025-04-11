using FastBuy.Payments.Contracts.Dtos;
using FastBuy.Payments.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastBuy.Payments.Api.Controllers
{
    [ApiController]
    [Route("payments")]
    public class PaymentsControllers :ControllerBase
    {
        private const string CustomerRole = "Customer";
        private readonly IPaymentServices paymentServices;

        public PaymentsControllers(IPaymentServices paymentServices)
        {
            this.paymentServices = paymentServices;
        }


        [HttpPost]
        [Authorize(Roles = CustomerRole)]
        public async Task<IActionResult> PaymentOrderAsync(CreatePaymentDto request)
        {
            return await paymentServices.Payment(request) ? Ok() : BadRequest();

        }


    }
}
