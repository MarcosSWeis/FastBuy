using FastBuy.Payments.Entities;

namespace FastBuy.Payments.Services
{
    public class PaymentProcessor
    {
        public PaymentStatus Procesar(decimal amount)
        {

            int entero = (int) Math.Floor(amount);

            int ultimoDigito = entero % 10;

            if (ultimoDigito % 2 == 0)
            {
                return PaymentStatus.Completed;
            } else if (ultimoDigito == 5)
            {
                return PaymentStatus.Pending;
            } else
            {
                return PaymentStatus.Rejected;
            }
        }
    }
}