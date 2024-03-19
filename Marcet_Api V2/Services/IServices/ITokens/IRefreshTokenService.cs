using Marcet_Api_V2.Models;
using Models.Dto.Token;

namespace Marcet_Api.Services.IServices.ITokens
{
    public interface IRefreshTokenService
    {
        Task<string> CreateRefreshToken(Guid userId, string tokenId);
        Task<TokensDTO?> RefreshAccessTokenDuringLogin(RefreshToken refreshToken);
        Task<TokensDTO?> RefreshAccessToken(TokensDTO tokensDTO);
    }
}
