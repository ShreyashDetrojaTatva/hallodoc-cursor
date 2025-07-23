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
                RoleId = user.AccountType == (int)HalloDoc.Common.Constants.AccountType.Admin && user.Admin != null ? user.Admin.RoleId :
                        user.AccountType == (int)HalloDoc.Common.Constants.AccountType.Physician && user.Physician != null ? user.Physician.RoleId : (int?)null,
                FirstName = user.AccountType == (int)HalloDoc.Common.Constants.AccountType.Admin && user.Admin != null ? user.Admin.FirstName :
                            user.AccountType == (int)HalloDoc.Common.Constants.AccountType.Physician && user.Physician != null ? user.Physician.FirstName :
                            user.AccountType == (int)HalloDoc.Common.Constants.AccountType.Patient && user.Patient != null ? user.Patient.FirstName : null,
                LastName = user.AccountType == (int)HalloDoc.Common.Constants.AccountType.Admin && user.Admin != null ? user.Admin.LastName :
                            user.AccountType == (int)HalloDoc.Common.Constants.AccountType.Physician && user.Physician != null ? user.Physician.LastName :
                            user.AccountType == (int)HalloDoc.Common.Constants.AccountType.Patient && user.Patient != null ? user.Patient.LastName : null
            };
        }
    }
} 