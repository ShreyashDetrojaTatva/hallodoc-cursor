using Microsoft.Extensions.Configuration;

namespace HalloDoc.Common.Constants
{
    public static class ConfigItems
    {
        public static IConfiguration Configuration { get; set; } = null!;
        public static string ConnectionString => Configuration["ConnectionStrings:DefaultConnection"] ?? "";
        public static bool IsDevelopmentMode => Convert.ToBoolean(Configuration["Data:IsDevelopmentMode"]);

        #region Email Sender Credentials
        public static string SmtpHost => Configuration["Email:SmtpHost"] ?? "";
        public static int SmtpPort => int.TryParse(Configuration["Email:SmtpPort"], out var port) ? port : 587;
        public static string SmtpUser => Configuration["Email:SmtpUser"] ?? "";
        public static string SmtpPass => Configuration["Email:SmtpPass"] ?? "";
        public static string SmtpFrom => Configuration["Email:SmtpFrom"] ?? "";
        #endregion

        #region JWT Configuration
        public static string JwtSecret => Configuration["Jwt:Secret"] ?? throw new Exception("JWT secret not configured");
        public static int JwtExpiryInMinutes => int.TryParse(Configuration["Jwt:ExpiryInMinutes"], out var minutes) ? minutes : 60;
        #endregion
    }
} 