using System.ComponentModel.DataAnnotations;

namespace FastBuy.Orders.Contracts.Dtos
{
    public record SubmitPurchaseDto(
        [Required] Guid? ItemId,
        [Range(1,100)] int Quantity,
        [Required] Guid CorrelationId);
}
