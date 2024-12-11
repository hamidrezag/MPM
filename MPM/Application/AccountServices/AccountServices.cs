using Infrastructure.Extensions;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.AccountServices
{
    public interface IAccountServices
    {
        public Task<string> GetToken(string phonenumber);
        
    }
    [Scoped]
    public class AccountServices : IAccountServices
    {
        public async Task<string> GetToken(string phonenumber)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your-256-bit-secret"));

            // Define the signing credentials
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Create the claims (optional)
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, "user_id"), // Subject
            new Claim(JwtRegisteredClaimNames.Email, "user@example.com"), // Email
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Unique token ID
        };

            // Define the token
            var token = new JwtSecurityToken(
                issuer: "your-issuer",
                audience: "your-audience",
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials
            );

            // Generate the token string
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }
    }
}
