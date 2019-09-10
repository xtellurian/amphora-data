using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Amphora.Api.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Amphora.Api.Services.Auth
{

    public class TokenAuthenticationService : IAuthenticateService
    {
        private readonly ISignInManager signInManager;
        private readonly IUserManager userManager;
        private readonly IOptionsMonitor<TokenManagementOptions> tokenManagement;
        private readonly ILogger<TokenAuthenticationService> logger;
        private static string secret;

        public TokenAuthenticationService(ISignInManager signInManager,
                                          IUserManager userManager,
                                          IOptionsMonitor<TokenManagementOptions> tokenManagement,
                                          ILogger<TokenAuthenticationService> logger)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.tokenManagement = tokenManagement;
            this.logger = logger;
            secret = tokenManagement.CurrentValue.Secret ?? (secret ?? RandomString(20));
        }

        public async Task<(bool success, string token)> GetToken(ClaimsPrincipal principal)
        {
            if (signInManager.IsSignedIn(principal))
            {
                var user = await userManager.GetUserAsync(principal);
                if (user == null) return (false, null);
                return (true, GenerateToken(user));
            }
            else
            {
                return (false, null);
            }
        }
        public async Task<(bool success, string token)> IsAuthenticated(TokenRequest request)
        {
            var token = string.Empty;
            var signInResult = await signInManager.PasswordSignInAsync(request.Username, request.Password, false, false);
            if (!signInResult.Succeeded) return (false, token);
            var user = await userManager.FindByNameAsync(request.Username);
            token = GenerateToken(user);
            return (true, token);
        }


        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        private string GenerateToken(IApplicationUser user)
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
                signingCredentials: credentials
            );
            token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            return token;
        }
    }
}