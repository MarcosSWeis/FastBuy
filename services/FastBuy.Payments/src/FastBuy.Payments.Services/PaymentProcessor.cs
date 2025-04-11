namespace FastBuy.Payments.Services
{
    public class PaymentProcessor
    {
        public bool Procesar(decimal amount)
        {
            // Extraer la parte entera
            int entero = (int) Math.Floor(amount);

            // Obtener el último dígito
            int ultimoDigito = entero % 10;

            // Si el último dígito es impar, falla
            bool esExitoso = ultimoDigito % 2 == 0;

            return esExitoso;
        }
    }
}