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
        /// <summary>
        /// Add new user to data base
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task CreateAsync(TUser user)
        {
            if (!(user == null))
            {
                dbContext.Users.Add(user);
                dbContext.SaveChanges();
            }            
            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Disable user, but dont delete
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task DeleteAsync(TUser user)
        {
            TUser _user = dbContext.Users.FirstOrDefault(o => o.Id == user.Id) as TUser;
            if (_user != null)
            {
                _user.IsDisabled = true;
                UpdateAsync(_user);
            }            
            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Dispose 
        /// </summary>
        public void Dispose()
        {
            dbContext.Dispose();
        }

        /// <summary>
        /// Find user by Id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task<TUser> FindByIdAsync(string userId)
        {
            TUser _user = dbContext.Users.FirstOrDefault(o => o.Id == userId) as TUser;
            return Task.FromResult<TUser>(_user);
        }

        /// <summary>
        /// Find user by name
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public Task<TUser> FindByNameAsync(string userName)
        {
            TUser _user = dbContext.Users.FirstOrDefault(o => o.UserName == userName) as TUser;
            return Task.FromResult<TUser>(_user);
        }

        /// <summary>
        /// Get hashed user password
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<string> GetPasswordHashAsync(TUser user)
        {
            String passwordHash = dbContext.Users.FirstOrDefault(o => o.Id == user.Id).PasswordHash;
            return Task.FromResult<String>(passwordHash);
        }

        /// <summary>
        /// True if user has  hashed password.
        /// Else false
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> HasPasswordAsync(TUser user)
        {
            Boolean hasPasswordHash = !String.IsNullOrEmpty(dbContext.Users.FirstOrDefault(o => o.Id == user.Id).PasswordHash);
            return Task.FromResult<Boolean>(hasPasswordHash);
        }

        /// <summary>
        /// Set hashed password for user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="passwordHash"></param>
        /// <returns></returns>
        public Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            TUser _user = dbContext.Users.FirstOrDefault(o => o.Id == user.Id) as TUser;
            if (user != null)
            {
                _user.PasswordHash = passwordHash;
                UpdateAsync(_user);
            }            
            return Task.FromResult<Object>(null);
        }          

        /// <summary>
        /// Update 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task UpdateAsync(TUser user)
        {
            TUser _user = dbContext.Users.FirstOrDefault(o => o.Id == user.Id) as TUser;

            if (_user != null)
            {
                _user = user;
                dbContext.SaveChanges();
            }            
            return Task.FromResult<Object>(null);
        }
        #endregion

        #region /////////////////////// IUserSecurityStampStore Impl ////////////////////////
        /// <summary>
        /// Get user security stamp
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<string> GetSecurityStampAsync(TUser user)
        {
            if (user != null)
            {
                return Task.FromResult(user.SecurityStamp);
            }            
            return Task.FromResult<String>(null);
        }

        /// <summary>
        /// Set user security stamp
        /// </summary>
        /// <param name="user"></param>
        /// <param name="stamp"></param>
        /// <returns></returns>
        public Task SetSecurityStampAsync(TUser user, string stamp)
        {
            if ((user != null) && (!String.IsNullOrEmpty(stamp)))
            {
                user.SecurityStamp = stamp;
                dbContext.SaveChanges();
            }

            return Task.FromResult<object>(null);
            //return Task.FromResult(0);
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
            if (_user != null)
            {
                _user.Email = email;
                UpdateAsync(_user);
            }
            return Task.FromResult<object>(null);
            //return Task.FromResult(0);
        }

        /// <summary>
        /// Get user email
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<string> GetEmailAsync(TUser user)
        {
            if (user != null)
            {
                return Task.FromResult<String>(user.Email);
            }
            return Task.FromResult<String>(null);
        }

        /// <summary>
        /// Get if user email is confirmed
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> GetEmailConfirmedAsync(TUser user)
        {
            if (user != null)
            {
                return Task.FromResult<Boolean>(user.EmailConfirmed);
            }
            return Task.FromResult<Boolean>(false);
        }

        /// <summary>
        /// Set when user email is confirmed
        /// </summary>
        /// <param name="user"></param>
        /// <param name="confirmed"></param>
        /// <returns></returns>
        public Task SetEmailConfirmedAsync(TUser user, bool confirmed)
        {
            if (user != null)
            {
                user.EmailConfirmed = confirmed;
                UpdateAsync(user);
            }
            return Task.FromResult<String>(null);
            //return Task.FromResult(0);
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
            if (_user != null)
            {
                _user.PhoneNumber = phoneNumber;
                UpdateAsync(_user);
            }
            return Task.FromResult<Object>(null);
            //return Task.FromResult(0);
        }

        /// <summary>
        /// Get user phone number
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<string> GetPhoneNumberAsync(TUser user)
        {
            if (user != null)
            {
                return Task.FromResult<String>(user.PhoneNumber);
            }
            return Task.FromResult<String>(null);
        }

        /// <summary>
        /// Get if user phone number is confirmed
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> GetPhoneNumberConfirmedAsync(TUser user)
        {
            if (user != null)
            {
                return Task.FromResult<Boolean>(user.PhoneNumberConfirmed);
            }
            return Task.FromResult<Boolean>(false);
        }

        /// <summary>
        /// Set phone number if confirmed
        /// </summary>
        /// <param name="user"></param>
        /// <param name="confirmed"></param>
        /// <returns></returns>
        public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed)
        {
            if (user != null)
            {
                user.PhoneNumberConfirmed = confirmed;
                UpdateAsync(user);
            }
            return Task.FromResult<Object>(null);
            //return Task.FromResult(0);
        }
        #endregion

        #region /////////////////////////// IUserLoginStore Impl ////////////////////////////
        /// <summary>
        /// Adds a login to the user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="login"></param>
        /// <returns></returns>
        public Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            if ((user != null) && (login != null))
            {
                TUser _user = dbContext.Users.FirstOrDefault(o => o.Id == user.Id) as TUser;
                _user.Logins.Add(
                    new UserLogin()
                    {
                        UserId = user.Id,
                        LoginProvider = login.LoginProvider,
                        ProviderKey = login.ProviderKey
                    }
                    );
                UpdateAsync(_user);
            }
            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Remove login from user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="login"></param>
        /// <returns></returns>
        public Task RemoveLoginAsync(TUser user, UserLoginInfo login)
        {
            TUser _user = dbContext.Users.FirstOrDefault(o => o.Id == user.Id) as TUser;
            if ((user != null) && (login != null))
            {
                UserLogin usrLgn = _user.Logins
                .FirstOrDefault(o => (o.LoginProvider == login.LoginProvider) &&
                (o.ProviderKey == login.ProviderKey));
                _user.Logins.Remove(usrLgn);
                UpdateAsync(_user);
            }            
            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Get user logininfo list
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
        {
            TUser _user = dbContext.Users.FirstOrDefault(o => o.Id == user.Id) as TUser;
            if (_user != null)
            {
                List<UserLoginInfo> logins = (from info in _user.Logins
                                              select new UserLoginInfo(info.LoginProvider, info.ProviderKey))
                                              .ToList<UserLoginInfo>();
                return Task.FromResult<IList<UserLoginInfo>>(logins);
            }
            return Task.FromResult<IList<UserLoginInfo>>(null);
        }

        /// <summary>
        /// Find user by login
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public Task<TUser> FindAsync(UserLoginInfo login)
        {
            TUser _user = null;
            if (login != null)
            {
                _user = (from user in dbContext.Users
                         from _login in user.Logins
                         where (_login.ProviderKey == login.ProviderKey) && (_login.LoginProvider == login.LoginProvider)
                         select user).FirstOrDefault() as TUser;
                return Task.FromResult<TUser>(_user);
            }
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
            if (user != null)
            {
                TUser _user = dbContext.Users.FirstOrDefault(o => o.Id == user.Id) as TUser;
                _user.TwoFactorEnabled = enabled;
                UpdateAsync(_user);
            }
            return Task.FromResult<object>(null);
           // return Task.FromResult(0);
        }

        /// <summary>
        /// Get if two factor authentication is 
        /// enabled on the user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> GetTwoFactorEnabledAsync(TUser user)
        {
            if (user != null)
            {
                return Task.FromResult<Boolean>(user.TwoFactorEnabled);
            }
            return Task.FromResult<Boolean>(false);
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
            if (user != null)
            {
                user.LockoutEndDateUtc = lockoutEnd.UtcDateTime;
                UpdateAsync(user);
            }
            return Task.FromResult<object>(null);
            //return Task.FromResult(0);
        }

        /// <summary>
        /// Increment failed access count
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<int> IncrementAccessFailedCountAsync(TUser user)
        {
            if (user != null)
            {
                user.AccessFailedCount++;
                UpdateAsync(user);
                return Task.FromResult(user.AccessFailedCount);
            }
            return Task.FromResult<Int32>(0);
        }

        /// <summary>
        /// Reset failed access count
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task ResetAccessFailedCountAsync(TUser user)
        {
            if (user != null)
            {
                user.AccessFailedCount = 0;
                UpdateAsync(user);
            }            
            return Task.FromResult(0);
        }

        /// <summary>
        /// Get failed access count
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<int> GetAccessFailedCountAsync(TUser user)
        {
            if (user != null)
            {
                return Task.FromResult<Int32>(user.AccessFailedCount);
            }
            return Task.FromResult<Int32>(0);
        }

        /// <summary>
        /// Get if lockout is enabled for the user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> GetLockoutEnabledAsync(TUser user)
        {
            if (user != null)
            {
                return Task.FromResult<Boolean>(user.LockoutEnabled);
            }
            return Task.FromResult<Boolean>(false);
        }

        /// <summary>
        /// Set lockout enabled for user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        public Task SetLockoutEnabledAsync(TUser user, bool enabled)
        {
            if (user != null)
            {
                user.LockoutEnabled = enabled;
                UpdateAsync(user);
            }
            return Task.FromResult<Object>(null);
            //return Task.FromResult(0);
        }
        #endregion
    }
}
