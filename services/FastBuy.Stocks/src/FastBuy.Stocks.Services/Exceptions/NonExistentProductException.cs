namespace FastBuy.Stocks.Services.Exceptions
{
    public class NonExistentProductException :Exception
    {
        public Guid ProdcutId { get; set; }

        public NonExistentProductException(Guid productItemId) : base($"Non existent product {productItemId}.")
        {
            this.ProdcutId = productItemId;
        }
    }
}
