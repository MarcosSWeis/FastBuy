using FastBuy.Products.Contracts.Dtos;
using FastBuy.Products.Entities;

namespace FastBuy.Products.Services.Mapping
{
    public static class MappingExtensions
    {
        public static ProductResponseDto MapToProductResponseDto(this Product product)
        {
            return new ProductResponseDto(product.Id,product.Name,product.Description,product.Price,product.CreatedAt);
        }

        public static ProductRequestDto MapToProductRequestDto(this Product product)
        {
            return new ProductRequestDto(product.Name,product.Description,product.Price);
        }

        public static Product MapToProduct(this ProductRequestDto request)
        {
            return new Product()
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price
            };
        }
    }
}
