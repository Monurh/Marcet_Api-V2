using FluentValidation;
using Microsoft.IdentityModel.Tokens;
using Models.Dto.Token;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace Marcet_Api.Validators
{
    public class TokensDTOValidator : AbstractValidator<TokensDTO>
    {
        private readonly IConfiguration _jwt;
        private readonly RSA rsa;
        private readonly JwtSecurityTokenHandler jwtSecurityHandler;

        public TokensDTOValidator(IConfiguration configuration, ILogger<TokensDTOValidator> logger)
        {
            _jwt = configuration.GetSection("JWT");

            rsa = RSA.Create();

            rsa.FromXmlString(_jwt.GetValue<string>("PublicKey"));

            jwtSecurityHandler = new JwtSecurityTokenHandler();

            RuleFor(x => x.AccessToken)
                .NotNull()
                .NotEmpty()
                .WithMessage("Invalid token");

            RuleFor(x => x.RefreshToken)
                .NotNull()
                .NotEmpty()
                .WithMessage("Invalid token");

            RuleFor(x => x.AccessToken)
                .MustAsync(IsValidAccessTokenSign)
                .WithMessage("Invalid token");
        }

        private async Task<bool> IsValidAccessTokenSign(string accessToken, CancellationToken token)
        {
            try
            {
                TokenValidationParameters validationParameters = new TokenValidationParameters()
                {
                    IssuerSigningKey = new RsaSecurityKey(rsa),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false,
                    ValidAlgorithms = new[] { SecurityAlgorithms.RsaSha256 },
                };

                TokenValidationResult principal = await jwtSecurityHandler.ValidateTokenAsync(accessToken, validationParameters);

                if (!principal.IsValid)
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
