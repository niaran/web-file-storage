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

        /// <summary>
        /// Default ctor
        /// </summary>
        public UserStore()
        {
            dbContext = new IdentityContext();
        }

        #region ////////////////////////////// IUserStore Impl //////////////////////////////
        public Task CreateAsync(TUser user)
        {
            dbContext.Users.Add(user);
            dbContext.SaveChanges();
            return Task.FromResult<object>(null);
        }

        public Task DeleteAsync(TUser user)
        {
            dbContext.Users.FirstOrDefault(o => o.Id == user.Id).IsDisabled = true;
            dbContext.SaveChanges();
            return Task.FromResult<object>(null);
        }

        public void Dispose()
        {
            dbContext.Dispose();
        }

        public Task<TUser> FindByIdAsync(string userId)
        {
            TUser _user = dbContext.Users.FirstOrDefault(o => o.Id == userId) as TUser;
            return Task.FromResult<TUser>(_user);
        }

        public Task<TUser> FindByNameAsync(string userName)
        {
            TUser _user = dbContext.Users.FirstOrDefault(o => o.UserName == userName) as TUser;
            return Task.FromResult<TUser>(_user);
        }

        public Task<string> GetPasswordHashAsync(TUser user)
        {
            String passwordHash = dbContext.Users.FirstOrDefault(o => o.Id == user.Id).PasswordHash;
            return Task.FromResult<String>(passwordHash);
        }

        public Task<bool> HasPasswordAsync(TUser user)
        {
            Boolean hasPasswordHash = String.IsNullOrEmpty(dbContext.Users.FirstOrDefault(o => o.Id == user.Id).PasswordHash);
            // добавить условие проверки
            return Task.FromResult<Boolean>(hasPasswordHash);
        }

        public Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            TUser _user = dbContext.Users.FirstOrDefault(o => o.Id == user.Id) as TUser;
            _user.PasswordHash = passwordHash;
            dbContext.SaveChanges();
            return Task.FromResult<Object>(null);
        }          

        public Task UpdateAsync(TUser user)
        {
            TUser _user = dbContext.Users.FirstOrDefault(o => o.Id == user.Id) as TUser;
            _user = user;
            dbContext.SaveChanges();
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
            dbContext.SaveChanges();
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
            TUser _user = dbContext.Users.FirstOrDefault(o => o.Id == user.Id) as TUser;
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
            TUser _user = dbContext.Users.FirstOrDefault(o => o.Email == email) as TUser;
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
            TUser _user = dbContext.Users.FirstOrDefault(o => o.Id == user.Id) as TUser;
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

        #region /////////////////////////// IUserLoginStore Impl ////////////////////////////
        public Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            // проверить юзера и логин
            dbContext.UserLogins
                .Add(
                new UserLogin() {
                    UserId = user.Id,
                    LoginProvider = login.LoginProvider,
                    ProviderKey = login.ProviderKey }
                );

            dbContext.SaveChanges();

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="login"></param>
        /// <returns></returns>
        public Task RemoveLoginAsync(TUser user, UserLoginInfo login)
        {
            UserLogin usLgn = dbContext.UserLogins
                .FirstOrDefault(o => (o.UserId == user.Id) &&
                (o.LoginProvider == login.LoginProvider) &&
                (o.ProviderKey == login.ProviderKey));

            dbContext.UserLogins.Remove(usLgn);
            dbContext.SaveChanges();
            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Get user logininfo list
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
        {
            List<UserLoginInfo> logins = (from ulinfo in dbContext.UserLogins
                                          where ulinfo.UserId == user.Id
                                          select new UserLoginInfo(ulinfo.LoginProvider, ulinfo.ProviderKey)).ToList<UserLoginInfo>();
            return Task.FromResult<IList<UserLoginInfo>>(logins);
        }

        /// <summary>
        /// Find user by login
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public Task<TUser> FindAsync(UserLoginInfo login)
        {
            String userId = dbContext.UserLogins
                .FirstOrDefault(o => (o.LoginProvider == login.LoginProvider) && 
                (o.ProviderKey == login.ProviderKey))
                .UserId;
            TUser _user = dbContext.Users.FirstOrDefault(o => o.Id == userId) as TUser;
            return Task.FromResult<TUser>(_user);
        }
        #endregion

        #region ///////////////////////// IUserTwoFactorStore Impl //////////////////////////
        /// <summary>
        /// Set two factor authentication for user (is enabled)
        /// </summary>
        /// <param name="user"></param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled)
        {
            TUser _user = dbContext.UserLogins.FirstOrDefault(o => o.UserId == user.Id) as TUser;
            _user.TwoFactorEnabled = enabled;
            UpdateAsync(_user);
            return Task.FromResult(0);
        }

        /// <summary>
        /// Get if two factor authentication is 
        /// enabled on the user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> GetTwoFactorEnabledAsync(TUser user)
        {
            return Task.FromResult<Boolean>(user.TwoFactorEnabled);
        }
        #endregion

        #region /////////////////////////// IUserLockoutStore Impl //////////////////////////
        /// <summary>
        /// Get user lock out end date
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<DateTimeOffset> GetLockoutEndDateAsync(TUser user)
        {
            return Task.FromResult(user.LockoutEndDateUtc.HasValue ? 
                new DateTimeOffset(DateTime.SpecifyKind(user.LockoutEndDateUtc.Value, DateTimeKind.Utc)) : 
                new DateTimeOffset());
        }

        /// <summary>
        /// Set user lockout end date
        /// </summary>
        /// <param name="user"></param>
        /// <param name="lockoutEnd"></param>
        /// <returns></returns>
        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset lockoutEnd)
        {
            user.LockoutEndDateUtc = lockoutEnd.UtcDateTime;
            UpdateAsync(user);
            return Task.FromResult(0);
        }

        /// <summary>
        /// Increment failed access count
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<int> IncrementAccessFailedCountAsync(TUser user)
        {
            user.AccessFailedCount++;
            UpdateAsync(user);
            return Task.FromResult(user.AccessFailedCount);
        }

        /// <summary>
        /// Reset failed access count
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task ResetAccessFailedCountAsync(TUser user)
        {
            user.AccessFailedCount = 0;
            UpdateAsync(user);
            return Task.FromResult(0);
        }

        /// <summary>
        /// Get failed access count
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<int> GetAccessFailedCountAsync(TUser user)
        {
            return Task.FromResult<Int32>(user.AccessFailedCount);
        }

        /// <summary>
        /// Get if lockout is enabled for the user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> GetLockoutEnabledAsync(TUser user)
        {
            return Task.FromResult<Boolean>(user.LockoutEnabled);
        }

        /// <summary>
        /// Set lockout enabled for user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        public Task SetLockoutEnabledAsync(TUser user, bool enabled)
        {
            user.LockoutEnabled = enabled;
            UpdateAsync(user);
            return Task.FromResult(0);
        }
        #endregion
    }
}
