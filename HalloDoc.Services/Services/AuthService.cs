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
using HalloDoc.Services.Helpers;

namespace HalloDoc.Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IWorkContext _workContext;
        public AuthService(IAuthRepository authRepository, IWorkContext workContext)
        {
            _authRepository = authRepository;
            _workContext = workContext;
        }

        public UserDetailsDto? ValidateUser(LoginViewModel login)
        {
            var userEntity = _authRepository.GetUserByUsernameOrEmail(
                login.UsernameOrEmail,
                q => q.Include(u => u.Admins).Include(u => u.Physicians).Include(u => u.Patients)
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

        public async Task<ProfileDto?> GetProfileAsync()
        {
            var contextUser = _workContext.CurrentUser();
            if (contextUser == null)
                return null;
            var user = await _authRepository.GetUserWithPatientAsync(contextUser.UserId);
            if (user == null)
                return null;

            // Get the first patient (assuming one user has one patient for now)
            var patient = user.Patients?.FirstOrDefault();
            if (patient == null)
                return null;

            return new ProfileDto
            {
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                Email = user.Email,
                Username = user.Username,
                PhoneNumber = user.PhoneNumber,
                DOB = patient.DOB ?? DateTime.Now,
                Address = patient.Address,
                City = patient.City,
                RegionId = patient.RegionId,
                ZipCode = patient.ZipCode
            };
        }

        public async Task<bool> UpdateProfileAsync(UpdateProfileDto dto)
        {
            var contextUser = _workContext.CurrentUser();
            if (contextUser == null)
                return false;

            // Get user and patient separately to avoid circular references
            var user = await _authRepository.GetUserByEmailAsync(contextUser.Email ?? string.Empty);
            if (user == null)
                return false;

            var patient = await _authRepository.GetPatientByUserIdAsync(contextUser.UserId);
            if (patient == null)
                return false;

            // Update user entity
            user.PhoneNumber = dto.PhoneNumber;
            user.UpdatedAt = DateTime.Now;
            var userUpdateResult = await _authRepository.UpdateUserAsync(user);
            if (!userUpdateResult)
                return false;

            // Update patient entity separately
            patient.FirstName = dto.FirstName;
            patient.LastName = dto.LastName;
            patient.DOB = dto.DOB;
            patient.Address = dto.Address;
            patient.City = dto.City;
            patient.RegionId = dto.RegionId;
            patient.ZipCode = dto.ZipCode;
            patient.UpdatedAt = DateTime.Now;

            return await _authRepository.UpdatePatientAsync(patient);
        }
    }
} 