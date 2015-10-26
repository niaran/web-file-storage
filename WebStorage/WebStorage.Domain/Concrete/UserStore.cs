using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using WebStorage.Domain.Entities;

namespace WebStorage.Domain.Concrete
{
    /// <summary>
    /// Class that implements the key ASP.NET 
    /// Identity user store iterfaces
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
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
        private IdentityContext dbContext;
        private List<TUser> _users;
        
        /// <summary>
        /// Defaul ctor
        /// </summary>
        public UserStore()
        {
            dbContext = new IdentityContext();
            _users = dbContext.Users as List<TUser>;
        }

        #region ////////////////////////////// IUserStore Impl //////////////////////////////
        public Task CreateAsync(TUser user)
        {
            _users.Add(user);
            return Task.FromResult<object>(null);
        }

        public Task DeleteAsync(TUser user)
        {
            _users.FirstOrDefault(o => o.Id == user.Id).IsDisabled = true;
            return Task.FromResult<object>(null);
        }

        public void Dispose()
        {
            dbContext.Dispose();
        }

        public Task<TUser> FindByIdAsync(string userId)
        {
            TUser _user = _users.FirstOrDefault(o => o.Id == userId) as TUser;
            return Task.FromResult<TUser>(_user);
        }

        public Task<TUser> FindByNameAsync(string userName)
        {
            TUser _user = _users.FirstOrDefault(o => o.UserName == userName) as TUser;
            return Task.FromResult<TUser>(_user);
        }

        public Task<string> GetPasswordHashAsync(TUser user)
        {
            String passwordHash = _users.FirstOrDefault(o => o.Id == user.Id).PasswordHash;
            return Task.FromResult<String>(passwordHash);
        }

        public Task<bool> HasPasswordAsync(TUser user)
        {
            Boolean hasPasswordHash = String.IsNullOrEmpty(_users.FirstOrDefault(o => o.Id == user.Id).PasswordHash);
            // добавить условие проверки
            return Task.FromResult<Boolean>(hasPasswordHash);
        }

        public Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            TUser _user = _users.FirstOrDefault(o => o.Id == user.Id) as TUser;
            _user.PasswordHash = passwordHash;
            return Task.FromResult<Object>(null);
        }

        public Task UpdateAsync(TUser user)
        {
            TUser _user = _users.FirstOrDefault(o => o.Id == user.Id) as TUser;
            _user = user;
            return Task.FromResult<Object>(null);
        }
        #endregion
    }
}
