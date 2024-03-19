namespace Marcet_Api_V2.Models;

public partial class RefreshToken
{
    public Guid? Id { get; set; }

    public Guid? UserId { get; set; }

    public string? AccessTokenId { get; set; }

    public string? Token { get; set; }

    public bool? IsValid { get; set; }

    public DateTime? ExpiresAt { get; set; }
}
