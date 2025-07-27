using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using HalloDoc.Common.Constants;
using HalloDoc.Repositories.Repositories.AuthRepository;
using HalloDoc.Entities.Data.Context;
using HalloDoc.Entities.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using HalloDoc.Repositories.DTOs;
using HalloDoc.Common.Helpers;
using HalloDoc.Services.ViewModels;
using HalloDoc.Repositories.Mappers;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

namespace HalloDoc.Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        public AuthService(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        public UserDetailsDto? ValidateUser(LoginViewModel login)
        {
            var userEntity = _authRepository.GetUserByUsernameOrEmail(
                login.UsernameOrEmail,
                q => q.Include(u => u.Admin).Include(u => u.Physician).Include(u => u.Patient)
            );
            // TODO: Replace with proper hashing (BCrypt) before production
            if (userEntity == null || userEntity.PasswordHash != login.Password)
                return null;
            return UserMapper.MapToUserDetailsDto(userEntity);
        }

        public string GenerateJwtToken(UserDetailsDto user)
        {
            return JwtHelper.GenerateJwtToken(user.UserId, user.AccountType, user.RoleId);
        }

        public async Task<string?> GenerateResetPasswordTokenAsync(string email)
        {
            var user = await _authRepository.GetUserByEmailAsync(email);
            if (user == null)
                return null;
            
            var resetToken = JwtHelper.GenerateResetPasswordToken(user.UserId, user.Email);
            var resetLink = $"http://localhost:4300/reset-password?token={resetToken}";
            return resetLink;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            // Validate the reset token
            var tokenValidation = JwtHelper.ValidateResetPasswordToken(resetPasswordDto.Token);
            if (tokenValidation == null)
                return false;

            var (userId, email) = tokenValidation.Value;

            // Verify the user exists and matches the token
            var user = await _authRepository.GetUserByEmailAsync(email);
            if (user == null || user.UserId != userId)
                return false;

            // TODO: Hash the new password (use common helper function if exists)            

            // Update the user's password
            return await _authRepository.UpdateUserPasswordAsync(userId, resetPasswordDto.Password);
        }
    }
} 