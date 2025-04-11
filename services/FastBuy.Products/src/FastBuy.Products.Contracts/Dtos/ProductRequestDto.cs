using System.ComponentModel.DataAnnotations;

namespace FastBuy.Products.Contracts.Dtos;

public record ProductRequestDto(
[Required]
string Name,
string Description,
[Range(0,int.MaxValue)]
decimal Price //se usa decimal ya que es mas preciso
);

