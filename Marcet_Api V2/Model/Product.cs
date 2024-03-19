using System;
using System.Collections.Generic;

namespace Marcet_Api_V2.Models;

public partial class Product
{
    public Guid ProductId { get; set; }

    public string? ProductName { get; set; }

    public string? Description { get; set; }

    public decimal? Price { get; set; }

    public int? StockQuantity { get; set; }

    public string? Category { get; set; }

    public string? Manufacturer { get; set; }

    public string? Photo { get; set; }

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
