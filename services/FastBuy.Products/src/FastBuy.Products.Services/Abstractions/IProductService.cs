using FastBuy.Products.Contracts.Dtos;

namespace FastBuy.Products.Services.Abstractions
{
    public interface IProductService
    {
        Task<IEnumerable<ProductResponseDto>> GetAsync();
        Task<ProductResponseDto> GetAsync(Guid id);
        Task<Guid> CreateAsync(ProductRequestDto request);
        Task UpdateAsync(Guid id,ProductRequestDto request);
        Task RemoveAsync(Guid id);
    }
}
