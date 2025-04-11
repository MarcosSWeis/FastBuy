namespace FastBuy.Products.Contracts.Events
{
    public record ProductCreated(Guid Id,string Name,String Description,decimal Price);
}
