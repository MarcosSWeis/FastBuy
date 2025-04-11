using FastBuy.Products.Contracts.Dtos;
using FastBuy.Products.Contracts.Events;
using FastBuy.Products.Entities;
using FastBuy.Products.Services.Abstractions;
using FastBuy.Products.Services.Mapping;
using FastBuy.Shared.Library.Repository.Abstractions;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.Logging;

namespace FastBuy.Products.Services.Implementations
{
    public class ProductService :IProductService
    {
        private readonly IRepository<Product> productRepository;
        private readonly ILogger<ProductService> _logger;
        private readonly IPublishEndpoint publishEndpoint;

        public ProductService(
            IRepository<Product> productRepository,
            ILogger<ProductService> logger,
            IPublishEndpoint publishEndpoint)
        {
            this.productRepository = productRepository;
            _logger = logger;
            this.publishEndpoint = publishEndpoint;
        }
        public async Task<Guid> CreateAsync(ProductRequestDto request)
        {
            _logger.LogInformation("Creando un producto.");

            var guid = await productRepository.CreateAsync(request.MapToProduct());

            await publishEndpoint.Publish(new ProductCreated(guid,request.Name,request.Description,request.Price));

            return guid;
        }

        public async Task<IEnumerable<ProductResponseDto>> GetAsync()
        {
            _logger.LogInformation("Obteniendo todos los productos.");

            return (await productRepository.GetAllAsync()).Select(x => x.MapToProductResponseDto());
        }

        public async Task<ProductResponseDto> GetAsync(Guid id)
        {
            _logger.LogInformation("Obteniendo un producto por id.");
            var product = await productRepository.GetAsync(id);

            if (product is null)
            {
                _logger.LogWarning($"El producto con id {id} no fue encontrado."); //solo los devs o it area
                throw new KeyNotFoundException($"El producto con id {id} no fue encontrado."); // el usuario final
            }

            return product.MapToProductResponseDto();
        }

        public async Task RemoveAsync(Guid id)
        {
            _logger.LogInformation("Obteniendo un producto por id.");

            var product = await productRepository.GetAsync(id);

            if (product is null)
            {
                _logger.LogWarning($"El producto con id {id} no fue encontrado.");
                throw new KeyNotFoundException($"El producto con id {id} no fue encontrado.");
            }

            _logger.LogInformation("Eliminado producto por id");

            await productRepository.RemoveAsync(id);

            await publishEndpoint.Publish(new ProductDeleted(id));
        }

        public async Task UpdateAsync(Guid id,ProductRequestDto request)
        {
            _logger.LogInformation("Obteniendo un producto por id.");

            var product = await productRepository.GetAsync(id) ??
                          throw new KeyNotFoundException($"El producto con id {id} no fue encontrado.");

            var entity = request.MapToProduct();
            entity.Id = id;

            _logger.LogInformation("Actualizando un porduto por id.");
            await productRepository.UpdateAsync(entity);

            await publishEndpoint.Publish(new ProductUpdated(id,request.Name,request.Description,request.Price));
        }
    }
}
