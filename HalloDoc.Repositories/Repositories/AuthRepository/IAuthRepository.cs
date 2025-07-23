using HalloDoc.Entities.Data.Entities;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace HalloDoc.Repositories.Repositories.AuthRepository
{
    public interface IAuthRepository
    {
        Users? GetUserByUsernameOrEmail(string usernameOrEmail, Func<IQueryable<Users>, IQueryable<Users>>? include = null);
    }
} 