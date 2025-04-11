using FastBuy.Payments.Contracts.Dtos;

namespace FastBuy.Payments.Services
{
    public interface IPaymentServices
    {
        Task<bool> Payment(CreatePaymentDto createPayment);


    }
}
