using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using System.Linq;
using MoneyManagerService.Models.DTOs;
using MoneyManagerService.Entities;
using MoneyManagerService.Models.Settings;
using MoneyManagerService.Data.Repositories;
using Google.Apis.Auth;
//using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace MoneyManagerService.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ServiceControllerBase
    {
        private readonly UserRepository userRepository;
        private readonly AuthenticationSettings authSettings;
        private readonly IMapper mapper;


        public AuthController(UserRepository userRepository, IOptions<AuthenticationSettings> authSettings, IMapper mapper)
        {
            this.userRepository = userRepository;
            this.authSettings = authSettings.Value;
            this.mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserLoginDto>> RegisterAsync([FromBody] RegisterUserDto userForRegisterDto)
        {
            var user = mapper.Map<User>(userForRegisterDto);

            var result = await userRepository.CreateUserWithPasswordAsync(user, userForRegisterDto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.Select(e => e.Description).ToList());
            }

            var token = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            user.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshToken,
                Expiration = DateTimeOffset.UtcNow.AddMinutes(authSettings.RefreshTokenExpirationTimeInMinutes),
                DeviceId = userForRegisterDto.DeviceId
            });

            await userRepository.SaveAllAsync();

            var userToReturn = mapper.Map<UserDto>(user);

            return CreatedAtRoute("GetUserAsync", new { controller = "Users", id = user.Id }, new UserLoginDto
            {
                Token = token,
                RefreshToken = refreshToken,
                User = userToReturn
            });
        }

        [HttpPost("register/google")]
        public async Task<ActionResult<UserLoginDto>> RegisterWithGoogleAccountAsync([FromBody] RegisterUserUsingGoolgleDto userForRegisterDto)
        {
            try
            {
                var validatedToken = await GoogleJsonWebSignature.ValidateAsync(userForRegisterDto.IdToken, new GoogleJsonWebSignature.ValidationSettings { Audience = new List<string> { "504553588506-joctqv1rhpn8o06apgdb2904qfi6fn26.apps.googleusercontent.com" } });

                var user = new User
                {
                    UserName = userForRegisterDto.UserName,
                    Email = validatedToken.Email,
                    EmailConfirmed = validatedToken.EmailVerified,
                    FirstName = validatedToken.GivenName,
                    LastName = validatedToken.FamilyName,
                    LinkedAccounts = new List<LinkedAccount>
                    {
                        new LinkedAccount
                        {
                            Id = validatedToken.Subject,
                            LinkedAccountType = LinkedAccountType.Google
                        }
                    }
                };

                var createResult = await userRepository.CreateUserWithAsync(user);

                if (!createResult.Succeeded)
                {
                    return BadRequest(createResult.Errors.Select(e => e.Description).ToList());
                }

                var token = GenerateJwtToken(user);
                var refreshToken = GenerateRefreshToken();

                user.RefreshTokens.Add(new RefreshToken
                {
                    Token = refreshToken,
                    Expiration = DateTimeOffset.UtcNow.AddMinutes(authSettings.RefreshTokenExpirationTimeInMinutes),
                    DeviceId = userForRegisterDto.DeviceId
                });

                await userRepository.SaveAllAsync();

                var userToReturn = mapper.Map<UserDto>(user);

                return CreatedAtRoute("GetUserAsync", new { controller = "Users", id = user.Id }, new UserLoginDto
                {
                    Token = token,
                    RefreshToken = refreshToken,
                    User = userToReturn
                });
            }
            catch (InvalidJwtException)
            {
                return Unauthorized("Invaid Id Token.");
            }
            catch
            {
                return InternalServerError("Unable to register using Google account.");
            }
        }

        /// <summary>
        /// Logs the user in
        /// </summary>
        /// <param name="userForLoginDto"></param>
        /// <returns>200 with user object on success. 401 on failure.</returns>
        [HttpPost("login")]
        public async Task<ActionResult<UserLoginDto>> LoginAsync([FromBody] LoginUserDto userForLoginDto)
        {
            var user = await userRepository.GetByUsernameAsync(userForLoginDto.Username, user => user.RefreshTokens);

            if (user == null)
            {
                return Unauthorized("Invalid username or password.");
            }

            var result = await userRepository.CheckPasswordAsync(user, userForLoginDto.Password);

            if (!result)
            {
                return Unauthorized("Invalid username or password.");
            }

            user.RefreshTokens.RemoveAll(token => token.Expiration <= DateTime.UtcNow || token.DeviceId == userForLoginDto.DeviceId);

            var token = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            user.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshToken,
                Expiration = DateTimeOffset.UtcNow.AddMinutes(authSettings.RefreshTokenExpirationTimeInMinutes),
                DeviceId = userForLoginDto.DeviceId
            });

            await userRepository.SaveAllAsync();

            var userToReturn = mapper.Map<UserDto>(user);

            return Ok(new UserLoginDto
            {
                Token = token,
                RefreshToken = refreshToken,
                User = userToReturn
            });
        }

        [HttpPost("login/google")]
        public async Task<ActionResult<UserLoginDto>> LoginGoogleAsync([FromBody] LoginGoogleUserDto userForLoginDto)
        {
            try
            {
                var validatedToken = await GoogleJsonWebSignature.ValidateAsync(userForLoginDto.IdToken, new GoogleJsonWebSignature.ValidationSettings { Audience = new List<string> { "504553588506-joctqv1rhpn8o06apgdb2904qfi6fn26.apps.googleusercontent.com" } });

                var user = await userRepository.GetByLinkedAccountAsync(validatedToken.Subject, LinkedAccountType.Google, user => user.RefreshTokens);

                if (user == null)
                {
                    return NotFound("No account found for this Google account.");
                }

                user.RefreshTokens.RemoveAll(token => token.Expiration <= DateTime.UtcNow || token.DeviceId == userForLoginDto.DeviceId);

                var token = GenerateJwtToken(user);
                var refreshToken = GenerateRefreshToken();

                user.RefreshTokens.Add(new RefreshToken
                {
                    Token = refreshToken,
                    Expiration = DateTimeOffset.UtcNow.AddMinutes(authSettings.RefreshTokenExpirationTimeInMinutes),
                    DeviceId = userForLoginDto.DeviceId
                });

                await userRepository.SaveAllAsync();

                var userToReturn = mapper.Map<UserDto>(user);

                return Ok(new UserLoginDto
                {
                    Token = token,
                    RefreshToken = refreshToken,
                    User = userToReturn
                });
            }
            catch (InvalidJwtException)
            {
                return Unauthorized("Invaid Id Token");
            }
            catch
            {
                return InternalServerError("Unable to login with Google.");
            }
        }

        [HttpPost("refreshToken")]
        public async Task<ActionResult> RefreshTokenAsync([FromBody] RefreshTokenDto refreshTokenDto)
        {
            // Still validate the passed in token, but ignore its expiration date by setting validate lifetime to false
            var validationParameters = new TokenValidationParameters
            {
                ClockSkew = TimeSpan.Zero,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(authSettings.APISecrect)),
                RequireSignedTokens = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                RequireExpirationTime = true,
                ValidateLifetime = false,
                ValidAudience = authSettings.TokenAudience,
                ValidIssuer = authSettings.TokenIssuer
            };

            ClaimsPrincipal tokenClaims;

            try
            {
                tokenClaims = new JwtSecurityTokenHandler().ValidateToken(refreshTokenDto.Token, validationParameters, out var rawValidatedToken);
            }
            catch (Exception e)
            {
                return Unauthorized(e.Message);
            }

            var userIdClaim = tokenClaims.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Invalid token.");
            }

            var user = await userRepository.GetByIdAsync(userId, user => user.RefreshTokens);

            if (user == null)
            {
                return Unauthorized("Invalid token.");
            }

            user.RefreshTokens.RemoveAll(token => token.Expiration <= DateTime.UtcNow);

            var currentRefreshToken = user.RefreshTokens.FirstOrDefault(token => token.DeviceId == refreshTokenDto.DeviceId);

            if (currentRefreshToken == null)
            {
                await userRepository.SaveAllAsync();
                return Unauthorized("Invalid token.");
            }

            user.RefreshTokens.Remove(currentRefreshToken);

            var token = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            user.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshToken,
                Expiration = DateTimeOffset.UtcNow.AddMinutes(authSettings.RefreshTokenExpirationTimeInMinutes),
                DeviceId = refreshTokenDto.DeviceId
            });

            await userRepository.SaveAllAsync();

            return Ok(new
            {
                token,
                refreshToken
            });
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
            };

            if (user.UserRoles != null)
            {
                foreach (string role in user.UserRoles.Select(r => r.Role.Name))
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings.APISecrect));

            if (key.KeySize < 512)
            {
                throw new Exception("API Secret must be longer");
            }

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(authSettings.TokenExpirationTimeInMinutes),
                NotBefore = DateTime.UtcNow,
                SigningCredentials = creds,
                Audience = authSettings.TokenAudience,
                Issuer = authSettings.TokenIssuer
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}