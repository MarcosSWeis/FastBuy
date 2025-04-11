namespace FastBuy.Products.Contracts.Events
{
    public record ProductUpdated(Guid Id,string Name,String Description,decimal Price);
}
