using Marcet_Api_V2.Models;

namespace Marcet_Api.Services.IServices.ITokens
{
    public interface IAccessTokenService
    {
        string CreateAccessToken(Customer user, string tokenId);
    }
}
