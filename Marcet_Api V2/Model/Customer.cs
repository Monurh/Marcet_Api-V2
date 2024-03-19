namespace Marcet_Api_V2.Models;

public partial class Customer
{
    public Guid CustomerId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Address { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Password { get; set; }

    public string? Rolle { get; set; }

    public string? Salt { get; set; }
}
