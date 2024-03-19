using System;
using System.Collections.Generic;

namespace Marcet_Api_V2.Models;

public partial class Order
{
    public Guid OrderId { get; set; }

    public DateTime? OrderDateTime { get; set; }

    public decimal? OrderTotal { get; set; }

    public string? OrderStatus { get; set; }

    public string? CustomerInformation { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Shipping> Shippings { get; set; } = new List<Shipping>();
}
