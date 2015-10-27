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
        IUserStore<TUser>
        where TUser : IdentityUser
    {
        private IdentityContext dbContext;
        private List<TUser> _users;
        
        /// <summary>
        /// Default ctor
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

        #region /////////////////////// IUserSecurityStampStore Impl ////////////////////////
        public Task<string> GetSecurityStampAsync(TUser user)
        {
            return Task.FromResult(user.SecurityStamp);
        }

        public Task SetSecurityStampAsync(TUser user, string stamp)
        {
            user.SecurityStamp = stamp;
            return Task.FromResult(0);
        }
        #endregion

        #region ////////////////////////////// IUserEmailStore //////////////////////////////
        /// <summary>
        /// Set user email
        /// </summary>
        /// <param name="user"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public Task SetEmailAsync(TUser user, string email)
        {
            TUser _user = _users.FirstOrDefault(o => o.Id == user.Id);
            _user.Email = email;
            UpdateAsync(_user);
            return Task.FromResult(0);
        }

        /// <summary>
        /// Get user email
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<string> GetEmailAsync(TUser user)
        {
            return Task.FromResult<String>(user.Email);
        }

        /// <summary>
        /// Get if user email is confirmed
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> GetEmailConfirmedAsync(TUser user)
        {
            return Task.FromResult<Boolean>(user.EmailConfirmed);
        }

        /// <summary>
        /// Set when user email is confirmed
        /// </summary>
        /// <param name="user"></param>
        /// <param name="confirmed"></param>
        /// <returns></returns>
        public Task SetEmailConfirmedAsync(TUser user, bool confirmed)
        {
            user.EmailConfirmed = confirmed;
            UpdateAsync(user);
            return Task.FromResult(0);
        }

        /// <summary>
        /// Find user by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public Task<TUser> FindByEmailAsync(string email)
        {
            TUser _user = _users.FirstOrDefault(o => o.Email == email);
            return Task.FromResult<TUser>(_user);
        }
        #endregion

        #region /////////////////////////// IUserPhoneNumberStore /////////////////////////// 
        /// <summary>
        /// Set user phone number
        /// </summary>
        /// <param name="user"></param>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public Task SetPhoneNumberAsync(TUser user, string phoneNumber)
        {
            TUser _user = _users.FirstOrDefault(o => o.Id == user.Id);
            _user.PhoneNumber = phoneNumber;
            UpdateAsync(_user);
            return Task.FromResult(0);
        }

        /// <summary>
        /// Get user phone number
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<string> GetPhoneNumberAsync(TUser user)
        {
            return Task.FromResult<String>(user.PhoneNumber);
        }

        /// <summary>
        /// Get if user phone number is confirmed
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> GetPhoneNumberConfirmedAsync(TUser user)
        {
            return Task.FromResult<Boolean>(user.PhoneNumberConfirmed);
        }

        /// <summary>
        /// Set phone number if confirmed
        /// </summary>
        /// <param name="user"></param>
        /// <param name="confirmed"></param>
        /// <returns></returns>
        public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed)
        {
            user.PhoneNumberConfirmed = confirmed;
            UpdateAsync(user);
            return Task.FromResult(0);
        }
        #endregion
    }
}
