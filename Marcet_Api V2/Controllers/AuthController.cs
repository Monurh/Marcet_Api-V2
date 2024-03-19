using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Marcet_Api.Models.Dto.Auth;
using Marcet_Api.Services.IServices;
using Marcet_Api.Services.IServices.ITokens;
using Marcet_Api.Validators.Auth;
using Models.Dto.Token;
using Models.Dto.Auth;

namespace Marcet_Api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IValidator<TokensDTO> _tokensDTOValidator;
        private readonly IAuthService _googleAuthService;
        private readonly IRefreshTokenService _refreshTokenService;


        public AuthController(IAuthService authService,
            IRefreshTokenService refreshTokenService,
            IValidator<TokensDTO> tokensDTOValidator)
        {
            _googleAuthService = authService;
            _refreshTokenService = refreshTokenService;
            _tokensDTOValidator = tokensDTOValidator;
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<TokensDTO>> GetNewTokenFromRefreshToken([FromBody] TokensDTO tokensDTO)
        {
            try
            {
                var isValidGoogleData = await _tokensDTOValidator.ValidateAsync(tokensDTO);

                if (!isValidGoogleData.IsValid)
                {
                    return BadRequest(isValidGoogleData.Errors);
                }

                var tokenDTOResponse = await _refreshTokenService.RefreshAccessToken(tokensDTO);

                if (tokenDTOResponse == null)
                {
                    return BadRequest("Invalid token");
                }

                return Ok(tokenDTOResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<TokensDTO>> Register([FromBody] RegistrationDTO registerRequest)
        {
            try
            {
                var validator = new RegistrationDTOValidator();
                var validationResult = await validator.ValidateAsync(registerRequest);

                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
                }

                var result = await _googleAuthService.RegisterAsync(registerRequest);

                if (result == null)
                {
                    return Conflict("User with this email already exists");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("authenticate")]
        public async Task<ActionResult<TokensDTO>> AuthenticateAsync([FromBody] LoginDTO loginRequest)
        {
            try
            {
                var validator = new LoginDTOValidator();
                var validationResult = await validator.ValidateAsync(loginRequest);

                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
                }

                var tokensDTO = await _googleAuthService.AuthenticateAsync(loginRequest);

                if (tokensDTO == null)
                {
                    return Unauthorized("Invalid email or password");
                }

                return Ok(tokensDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}