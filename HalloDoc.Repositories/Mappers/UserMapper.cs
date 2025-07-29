using HalloDoc.Entities.Data.Entities;
using HalloDoc.Repositories.DTOs;

namespace HalloDoc.Repositories.Mappers
{
    public static class UserMapper
    {
        public static UserDetailsDto MapToUserDetailsDto(Users user)
        {
            return new UserDetailsDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                AccountType = user.AccountType,
                RoleId = user.AccountType == (int)HalloDoc.Common.Constants.AccountType.Admin && user.Admins?.Any() == true ? user.Admins.First().RoleId :
                        user.AccountType == (int)HalloDoc.Common.Constants.AccountType.Physician && user.Physicians?.Any() == true ? user.Physicians.First().RoleId : (int?)null,
                FirstName = user.AccountType == (int)HalloDoc.Common.Constants.AccountType.Admin && user.Admins?.Any() == true ? user.Admins.First().FirstName :
                            user.AccountType == (int)HalloDoc.Common.Constants.AccountType.Physician && user.Physicians?.Any() == true ? user.Physicians.First().FirstName :
                            user.AccountType == (int)HalloDoc.Common.Constants.AccountType.Patient && user.Patients?.Any() == true ? user.Patients.First().FirstName : null,
                LastName = user.AccountType == (int)HalloDoc.Common.Constants.AccountType.Admin && user.Admins?.Any() == true ? user.Admins.First().LastName :
                            user.AccountType == (int)HalloDoc.Common.Constants.AccountType.Physician && user.Physicians?.Any() == true ? user.Physicians.First().LastName :
                            user.AccountType == (int)HalloDoc.Common.Constants.AccountType.Patient && user.Patients?.Any() == true ? user.Patients.First().LastName : null
            };
        }
    }
} 