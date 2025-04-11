﻿using FastBuy.Shared.Library.Repository.Abstractions;

namespace FastBuy.Orders.Entities
{
    public class ProductItem :IBaseEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }
}
