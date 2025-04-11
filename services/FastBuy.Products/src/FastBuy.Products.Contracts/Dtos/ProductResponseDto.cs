namespace FastBuy.Products.Contracts.Dtos;

public record ProductResponseDto
(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    DateTimeOffset CreatedAt     //no uso datetime ya ue usa me zona horaria local de mi pc, este usa el utc now
);
