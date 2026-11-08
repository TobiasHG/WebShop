namespace WebShop.Services
{
    public class PaymentService
    {
        public async Task<bool> ProcessPayment(string cardName, string cardNumber, string cardExpiry, string cardCvc, decimal totalAmount)
        {
            // TODO: Tilføj trejdeparts betaling
            await Task.Delay(1);
            return totalAmount > 0;
        }
    }
}