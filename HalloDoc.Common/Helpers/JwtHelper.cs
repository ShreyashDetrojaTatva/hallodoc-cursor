using HalloDoc.Common.Constants;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Linq;

namespace HalloDoc.Common.Helpers
{
    public static class JwtHelper
    {
        public static string GenerateJwtToken(int userId, int accountType, int? roleId)
        {
            var claims = new[]
            {
                new Claim("UserId", userId.ToString()),
                new Claim("AccountType", accountType.ToString()),
                new Claim("RoleId", roleId?.ToString() ?? string.Empty)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigItems.JwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(ConfigItems.JwtExpiryInMinutes),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static string GenerateResetPasswordToken(int userId, string email)
        {
            var claims = new[]
            {
                new Claim("UserId", userId.ToString()),
                new Claim("Email", email),
                new Claim("Type", "ResetPassword")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigItems.JwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static (int userId, string email)? ValidateResetPasswordToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigItems.JwtSecret));
                
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                var jwtToken = (JwtSecurityToken)validatedToken;

                // Check if this is a reset password token
                var typeClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == "Type")?.Value;
                if (typeClaim != "ResetPassword")
                    return null;

                var userIdClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value;
                var emailClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == "Email")?.Value;

                if (int.TryParse(userIdClaim, out var userId) && !string.IsNullOrEmpty(emailClaim))
                {
                    return (userId, emailClaim);
                }

                return null;
            }
            catch
            {
                return null;
            }
        }
    }
} 