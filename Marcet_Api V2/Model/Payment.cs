using System;
using System.Collections.Generic;

namespace Marcet_Api_V2.Models;

public partial class Payment
{
    public Guid PaymentId { get; set; }

    public Guid? OrderId { get; set; }

    public string? PaymentMethod { get; set; }

    public decimal? PaymentAmount { get; set; }

    public DateTime? PaymentDateTime { get; set; }

    public virtual Order? Order { get; set; }
}
