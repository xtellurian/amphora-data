using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Amphora.Api.Options;
using Amphora.Common.Models.Users;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Amphora.Api.Services.Auth
{
    public class TokenAuthenticationService : IAuthenticateService
    {
        private readonly IUserService userService;
        private readonly IOptionsMonitor<TokenManagementOptions> tokenManagement;
        private readonly ILogger<TokenAuthenticationService> logger;
        private static string secret;

        public TokenAuthenticationService(IUserService userService,
                                          IOptionsMonitor<TokenManagementOptions> tokenManagement,
                                          ILogger<TokenAuthenticationService> logger)
        {
            this.userService = userService;
            this.tokenManagement = tokenManagement;
            this.logger = logger;
            secret = tokenManagement.CurrentValue.Secret ?? (secret ?? RandomString(20));
        }

        public async Task<(bool success, string token)> GetToken(ClaimsPrincipal principal)
        {
            var user = await userService.ReadUserModelAsync(principal);
            if (user == null) { return (false, null); }
            using (logger.BeginScope(new LoggerScope<TokenAuthenticationService>(user)))
            {
                if (userService.IsSignedIn(principal))
                {
                    logger.LogInformation("Generating Token");
                    return (true, GenerateToken(user));
                }
                else
                {
                    logger.LogInformation("User Not Signed In");
                    return (false, null);
                }
            }
        }

        public async Task<(bool success, string token)> GetToken(TokenRequest request)
        {
            var user = await userService.UserManager.FindByNameAsync(request.Username);
            if (user == null) { return (false, null); }
            using (logger.BeginScope(new LoggerScope<TokenAuthenticationService>(user)))
            {
                var token = string.Empty;

                var signInResult = await userService.PasswordSignInAsync(request.Username, request.Password, false, false, true);
                if (!signInResult.Succeeded) { return (false, token); }
                token = GenerateToken(user);
                logger.LogInformation("Generated token. User Is Authenticated");
                return (true, token);
            }
        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private string GenerateToken(ApplicationUser user)
        {
            if (user == null)
            {
                throw new NullReferenceException("Cannot generate token for null user");
            }

            logger.LogInformation($"Generating token for {user.UserName}");
            string token;
            var claim = new[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwtToken = new JwtSecurityToken(
                tokenManagement.CurrentValue.Issuer,
                tokenManagement.CurrentValue.Audience,
                claim,
                expires: DateTime.Now.AddMinutes(tokenManagement.CurrentValue.AccessExpiration),
                signingCredentials: credentials);

            token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            return token;
        }
    }
}