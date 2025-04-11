using FastBuy.Stocks.Contracs.Dtos;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace FastBuy.Stocks.Services.Clients
{
    public class ProdutcClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProdutcClient> _logger;
        public ProdutcClient(HttpClient httpClient,ILogger<ProdutcClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }


        public async Task<ProductInfoDto?> GetProductByIdAsync(Guid productId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/products/{productId}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ProductInfoDto?>();
                }

                throw new HttpRequestException($"Error al obtener el producto: {productId}. Codigo de respuesta: {response.StatusCode}.");


            } catch (Exception ex)
            {
                _logger.LogError($@"Error al comunicarse con el microservicio de Productos. - {ex.Message} - {DateTimeOffset.UtcNow.ToString("dd/MM/yyyy HH:mm:ss")}");

                throw new ApplicationException("Error al comunicarse con el microservicio de Productos.");
            }
        }

    }
}
