using AutoMapper;
using Marcet_Api_V2.Models;
using Marcet_Api.Services.IServices.ITokens;
using Marcet_Api.Services.IServices;
using Microsoft.EntityFrameworkCore;
using Marcet_Api_V2.Repository;
using Services.IServices;
using Repository.IRepository;
using Models.Dto.Token;
using Models.Dto.Auth;
using Marcet_Api.Models.Dto.Auth;

public class AuthService : IAuthService
{
    private readonly MarcetDbContext _context;
    private readonly ICustomerService _userService;
    private readonly IAccessTokenService _accessTokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IRepository<RefreshToken> _refreshTokenRepository;
    private readonly IHashService _hashService;

    public AuthService(ICustomerService userService,
        IMapper mapper,
        IAccessTokenService accessTokenService,
        IHashService hashService,
        IRefreshTokenService refreshTokenService,
        IRepository<RefreshToken> refreshTokenRepository,
        MarcetDbContext context)
    {
        _userService = userService;
        _accessTokenService = accessTokenService;
        _refreshTokenService = refreshTokenService;
        _refreshTokenRepository = refreshTokenRepository;
        _hashService = hashService;
        _context = context;
    }
    public async Task<TokensDTO?> AuthenticateAsync(LoginDTO loginRequest)
    {
        try
        {
            Customer user = await _userService.GetCustomerByEmailAsync(loginRequest.Email);

            if (user != null && _hashService.VerifyPassword(loginRequest.Password, user.Password, user.Salt))
            {
                var tokenId = "JTI" + Guid.NewGuid().ToString();
                var accessToken = _accessTokenService.CreateAccessToken(user, tokenId);
                var refreshToken = await _refreshTokenService.CreateRefreshToken(user.CustomerId, tokenId);

                return new TokensDTO
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                };
            }

            return null;
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<TokensDTO?> RegisterAsync(RegistrationDTO registerRequest)
    {
        try
        {
            var existingUser = await _context.Customers.FirstOrDefaultAsync(u => u.Email == registerRequest.Email);

            if (existingUser != null)
            {
                return null;
            }


            if (registerRequest.Password != registerRequest.ConfirmPassword)
            {
                throw new ArgumentException("Passwords do not match.");
            }

            string salt = _hashService.GenerateSalt();
            string hashedPassword = _hashService.HashPassword(registerRequest.Password, salt);

            var newUser = new Customer
            {
                CustomerId = Guid.NewGuid(),
                Email = registerRequest.Email,
                Password = hashedPassword,
                Salt = salt,
                Rolle = "User",
            };

            _context.Customers.Add(newUser);
            await _context.SaveChangesAsync();

            var tokenId = "JTI" + Guid.NewGuid().ToString();
            var accessToken = _accessTokenService.CreateAccessToken(newUser, tokenId);
            var refreshToken = await _refreshTokenService.CreateRefreshToken(newUser.CustomerId, tokenId);

            return new TokensDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
        catch (Exception ex)
        {
            throw new Exception("Error while registering user", ex);
        }
    }
}
