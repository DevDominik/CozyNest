using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CozyNest.Classes
{
    public class GenerateToken
    {
        public static string GenerateAccessToken(string userId, string username)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Your_Secret_Key_Here")); // Use a secure key
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.UniqueName, username),
                new Claim(ClaimTypes.Name, username),
            };

            var token = new JwtSecurityToken(
                issuer: "cozynest",
                audience: "cozynest",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15), // Short-lived token
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Generate Refresh Token
        public static string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }
    }
}
