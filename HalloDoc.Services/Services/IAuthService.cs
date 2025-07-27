using HalloDoc.Repositories.DTOs;
using HalloDoc.Services.ViewModels;

namespace HalloDoc.Services.Services
{
    public interface IAuthService
    {
        UserDetailsDto? ValidateUser(LoginViewModel login);
        string GenerateJwtToken(UserDetailsDto user);
        Task<string?> GenerateResetPasswordTokenAsync(string email);
        Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
    }
} 