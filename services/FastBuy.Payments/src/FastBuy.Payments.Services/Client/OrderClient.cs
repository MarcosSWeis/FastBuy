

using FastBuy.Payments.Contracts.Dtos;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace FastBuy.Payments.Services.Client
{
    public class OrderClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<OrderClient> _logger;
        public OrderClient(HttpClient httpClient,ILogger<OrderClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }


        public async Task<OrderInfoDto?> GetStatusOrderByCorrelationIdAsync(Guid orderId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"orders/{orderId}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<OrderInfoDto?>();
                }

                throw new HttpRequestException($"Error el id de la ordern {orderId} no es valido para una orden.");


            } catch (Exception ex)
            {
                _logger.LogError($@"Error al comunicarse con el microservicio de Orders. - {ex.Message} - {DateTimeOffset.UtcNow.ToString("dd/MM/yyyy HH:mm:ss")}");

                throw new ApplicationException("Error al comunicarse con el microservicio de Orders.");
            }
        }

    }
}
