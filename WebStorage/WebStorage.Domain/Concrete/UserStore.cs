using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using WebStorage.Domain.Entities;

namespace WebStorage.Domain.Concrete
{
    class UserStore<TUser> : IUserPasswordStore<TUser>,
        IUserSecurityStampStore<TUser>,
        IUserEmailStore<TUser>,
        IUserPhoneNumberStore<TUser>,
        IUserLoginStore<TUser>,
        IUserTwoFactorStore<TUser, String>,
        IUserLockoutStore<TUser, String>,
        IUserLoginStore<TUser>,
        IUserStore<TUser>
        where TUser : IdentityUser
    {
        IdentityUserContext dbContext = new IdentityUserContext();

        public Task CreateAsync(TUser user)
        {
            dbContext.Users.Add(user);
        }

        public Task DeleteAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<TUser> FindByIdAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<TUser> FindByNameAsync(string userName)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetPasswordHashAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasPasswordAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(TUser user)
        {
            throw new NotImplementedException();
        }
    }
}
