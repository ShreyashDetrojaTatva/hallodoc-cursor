using System;
using System.Security.Cryptography;
using System.Text;

namespace HalloDoc.Common.Helpers
{
    public static class AgreementHelper
    {
        public static string GenerateAgreementToken(int requestId)
        {
            // Generate a unique token for the agreement
            var timestamp = DateTime.Now.Ticks;
            var randomBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }

            var tokenData = $"{requestId}_{timestamp}_{Convert.ToBase64String(randomBytes)}";
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(tokenData));
        }

        public static bool ValidateAgreementToken(string token, out int requestId)
        {
            requestId = 0;
            try
            {
                var decodedToken = Encoding.UTF8.GetString(Convert.FromBase64String(token));
                var parts = decodedToken.Split('_');
                if (parts.Length >= 3 && int.TryParse(parts[0], out requestId))
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public static DateTime GetAgreementExpiry()
        {
            // Agreements expire after 7 days
            return DateTime.Now.AddDays(7);
        }
    }
}