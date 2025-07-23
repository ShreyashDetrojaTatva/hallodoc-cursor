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
using HalloDoc.Services.Helpers;
using HalloDoc.Services.ViewModels;
using HalloDoc.Repositories.Mappers;

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
            return JwtHelper.GenerateJwtToken(user);
        }
    }
} 