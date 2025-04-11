namespace FastBuy.Payments.Services.Exceptions
{
    [Serializable]
    public class UnknowItemExceptions :Exception
    {
        public Guid itemID { get; }
        public UnknowItemExceptions(Guid itemId) : base($"Not found or unknown item '{itemId}'")
        {
            this.itemID = itemId;
        }
    }
}
