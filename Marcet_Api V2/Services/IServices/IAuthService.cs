using Marcet_Api.Models.Dto.Auth;
using Models.Dto.Auth;
using Models.Dto.Token;

namespace Marcet_Api.Services.IServices
{
    public interface IAuthService
    {
        public Task<TokensDTO?> AuthenticateAsync(LoginDTO loginRequest);
        public Task<TokensDTO?> RegisterAsync(RegistrationDTO registerRequest);
    }
}
