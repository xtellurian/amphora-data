using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Amphora.Api.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Amphora.Api.Services
{

    public class TokenAuthenticationService : IAuthenticateService
    {
        private readonly ISignInManager<ApplicationUser> signInManager;
        private readonly IUserManager<ApplicationUser> userManager;
        private readonly IOptionsMonitor<TokenManagementOptions> tokenManagement;

        public TokenAuthenticationService(ISignInManager<ApplicationUser> signInManager,
                                          IUserManager<ApplicationUser> userManager,
                                          IOptionsMonitor<TokenManagementOptions> tokenManagement)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.tokenManagement = tokenManagement;
        }

        public async Task<(bool success, string token)> GetToken(ClaimsPrincipal user)
        {
            if (signInManager.IsSignedIn(user))
            {
                var obj = await userManager.GetUserAsync(user);
                return (true, GenerateToken(obj.UserName));
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

            token = GenerateToken(request.Username);
            return (true, token);
        }

        private string GenerateToken(string username)
        {
            string token;
            var claim = new[]
{
                new Claim(ClaimTypes.Name, username)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenManagement.CurrentValue.Secret));
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