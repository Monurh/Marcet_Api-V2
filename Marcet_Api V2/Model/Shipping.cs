namespace Marcet_Api_V2.Models;

public partial class Shipping
{
    public Guid ShippingId { get; set; }

    public Guid? OrderId { get; set; }

    public string? ShippingMethod { get; set; }

    public DateOnly? DeliveryDate { get; set; }

    public string? DeliveryStatus { get; set; }

    public virtual Order? Order { get; set; }
}
