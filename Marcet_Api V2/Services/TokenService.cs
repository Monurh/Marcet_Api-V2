using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Marcet_Api_V2.Models;
using Microsoft.IdentityModel.Tokens;
using Marcet_Api.Services.IServices.ITokens;
using Repository.IRepository;
using Services.IServices;
using Models.Dto.Token;
using Models;

namespace Marcet_Api.Services
{
    public class TokenService : IAccessTokenService, IRefreshTokenService
    {
        private readonly IConfiguration _jwt;
        private readonly IConfiguration _refreshToken;
        private readonly IRepository<RefreshToken> _refreshTokenRepository;
        private readonly ICustomerService _userService;

        public TokenService(IConfiguration configuration,
            IRepository<RefreshToken> refreshTokenRepository,
            ICustomerService userService)
        {
            _jwt = configuration.GetSection("JWT");
            _refreshToken = configuration.GetSection("RefreshToken");

            _refreshTokenRepository = refreshTokenRepository;
            _userService = userService;
        }

        public string CreateAccessToken(Customer user, string tokenId)
        {
            try
            {
                using RSA rsa = RSA.Create();
                rsa.FromXmlString(_jwt.GetValue<string>("PrivateKey"));

                var securityKey = new RsaSecurityKey(rsa);

                var signingCredentials = new SigningCredentials(
                    key: securityKey,
                    algorithm: SecurityAlgorithms.RsaSha256)
                {
                    CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
                };

                var jwt = new JwtSecurityToken(
                    claims: new Claim[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, user.CustomerId.ToString()),
                        new Claim(JwtRegisteredClaimNames.Jti, tokenId),
                    },
                    issuer: _jwt.GetValue<string>("Issuer"),
                    notBefore: DateTime.Now,
                    expires: DateTime.Now.AddMinutes(int.Parse(_jwt.GetValue<string>("ExpirationTimeInMinutes"))),
                    signingCredentials: signingCredentials
                );

                var securityTokenHandler = new JwtSecurityTokenHandler();
                return securityTokenHandler.WriteToken(jwt);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<string> CreateRefreshToken(Guid userId, string tokenId)
        {
            try
            {
                RefreshToken refreshToken = new RefreshToken()
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    IsValid = true,
                    ExpiresAt = DateTime.UtcNow.AddDays(int.Parse(_refreshToken.GetValue<string>("ExpirationTimeInDays"))),
                    AccessTokenId = tokenId,
                    Token = Guid.NewGuid().ToString(),
                };

                await _refreshTokenRepository.CreateAsync(refreshToken);

                return refreshToken.Token;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<TokensDTO?> RefreshAccessTokenDuringLogin(RefreshToken refreshToken)
        {
            return await RefreshAccessToken(refreshToken);
        }

        public async Task<TokensDTO?> RefreshAccessToken(TokensDTO tokensDTO)
        {
            try
            {
                AccessTokenData accessTokenData = GetAccessTokenData(tokensDTO.AccessToken);

                var existingRefreshToken = await _refreshTokenRepository.GetAsync(t => t.AccessTokenId == accessTokenData.TokenId);

                if (existingRefreshToken == null)
                {
                    return null;
                }

                if (!IsValidAccessAndRefreshTokensData(accessTokenData, tokensDTO.RefreshToken, existingRefreshToken))
                {
                    existingRefreshToken.IsValid = false;
                    await _refreshTokenRepository.UpdateAsync(existingRefreshToken);

                    return null;
                }

                return await RefreshAccessToken(existingRefreshToken);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task<TokensDTO?> RefreshAccessToken(RefreshToken existingRefreshToken)
        {
            try
            {
                await RenewRefreshToken(existingRefreshToken);

                var user = await _userService.GetCustomerByIdAsync(existingRefreshToken.UserId.Value);

                if (user == null)
                {
                    return null;
                }

                return new TokensDTO()
                {
                    AccessToken = CreateAccessToken(user, existingRefreshToken.AccessTokenId),
                    RefreshToken = existingRefreshToken.Token,
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task RenewRefreshToken(RefreshToken existingRefreshToken)
        {
            try
            {
                existingRefreshToken.IsValid = true;
                existingRefreshToken.Token = Guid.NewGuid().ToString();
                existingRefreshToken.ExpiresAt = DateTime.UtcNow.AddDays(int.Parse(_refreshToken.GetValue<string>("ExpirationTimeInDays")));

                await _refreshTokenRepository.UpdateAsync(existingRefreshToken);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private AccessTokenData GetAccessTokenData(string accessToken)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwt = tokenHandler.ReadJwtToken(accessToken);

                var issuer = jwt.Claims.FirstOrDefault(c => c.Type == "iss").Value;
                var jwtTokenId = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti).Value;
                var userId = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub).Value;

                return new AccessTokenData()
                {
                    UserId = userId,
                    Issuer = issuer,
                    TokenId = jwtTokenId,
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool IsValidAccessAndRefreshTokensData(AccessTokenData accessTokenData, string refreshToken, RefreshToken existingRefreshToken)
        {
            try
            {
                return accessTokenData.UserId == existingRefreshToken.UserId.ToString() &&
                   accessTokenData.TokenId == existingRefreshToken.AccessTokenId &&
                   refreshToken == existingRefreshToken.Token &&
                   accessTokenData.Issuer == _jwt.GetValue<string>("Issuer") &&
                   existingRefreshToken.IsValid == true &&
                   existingRefreshToken.ExpiresAt >= DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
