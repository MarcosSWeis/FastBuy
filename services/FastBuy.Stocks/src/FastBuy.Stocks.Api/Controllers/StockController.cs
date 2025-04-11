using FastBuy.Stocks.Contracs.Dtos;
using FastBuy.Stocks.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FastBuy.Stocks.Api.Controllers
{
    [ApiController]
    [Route("stocks")]

    public class StockController :ControllerBase
    {
        private const string AdminRole = "Admin";
        private readonly IStockService _stockService;
        public StockController(IStockService stockService)
        {
            _stockService = stockService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetStockByProductId(Guid productId)
        {
            if (productId.Equals(Guid.Empty))
            {
                return BadRequest();
            }
            //authorization logic
            var currentUserId = User.FindFirstValue(JwtRegisteredClaimNames.Sub); //obtengo el id incluido en los claims del token
            if (!User.IsInRole(AdminRole))
            {
                return Forbid();
            }

            var response = await _stockService.GetStock(productId);

            return response is not null ? Ok(response) : NotFound();
        }

        [HttpPost]
        [Authorize(Roles = AdminRole)]
        public async Task<IActionResult> SetStock(Guid productId,int stock)
        {
            return await _stockService.SetStock(productId,stock) ? Ok() : BadRequest();
        }

        [HttpPut]
        [Authorize(Roles = AdminRole)]
        public async Task<IActionResult> UpdateStock(DecreaseStockRequestDto requestDto)
        {
            return await _stockService.DecreaseStock(requestDto) ? Ok() : BadRequest();
        }

    }
}
