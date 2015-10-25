using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace WebStorage.Domain.Entities
{
    class IdentityUser : IUser
    {

       /* #region ///////////////////// Ctors /////////////////////
        /// <summary>
        /// Default ctor
        /// </summary>
        public IdentityUser()
        {
            Id = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Ctor that takes user name as argument
        /// </summary>
        /// <param name="userName"></param>
        public IdentityUser(String userName) : this()
        {
            UserName = userName;
        }
        #endregion*/

        #region ///////////////////// Properties /////////////////////
        /// <summary>
        /// User ID
        /// </summary>
        public String Id { get; } = Guid.NewGuid().ToString();

        /// <summary>
        /// User Name
        /// </summary>
        public String UserName { get; set; }

        /// <summary>
        /// User Email
        /// </summary>
        public virtual String Email { get; set; }

        /// <summary>
        /// True if the email is confirmed, 
        /// default is false
        /// </summary>
        public virtual Boolean EmailConfirmed { get; set; }

        /// <summary>
        /// The hashed form of the user password
        /// </summary>
        public virtual String PasswordHash { get; set; }

        /// <summary>
        /// A random value that should change whenever a 
        /// users credentials have changed (password changed, login removed)
        /// </summary>
        public virtual String SecurityStamp { get; set; }

        /// <summary>
        /// PhoneNumber for the user
        /// </summary>
        public virtual String PhoneNumber { get; set; }

        /// <summary>
        /// True if the phone number is confirmed, 
        /// default is false
        /// </summary>
        public virtual Boolean PhoneNumberConfirmed { get; set; }

        /// <summary>
        /// Is two factor enabled for the user
        /// </summary>
        public virtual Boolean TwoFactorEnabled { get; set; }

        /// <summary>
        /// DateTime in UTC when lockout ends, any 
        /// time in the past is considered not locked out
        /// </summary>
        public virtual DateTime? LockoutEndDateUtc { get; set; }

        /// <summary>
        /// Is lockout enabled for this user
        /// </summary>
        public virtual Boolean LockoutEnabled { get; set; }

        /// <summary>
        /// Used to record failures for the purposes of lockout
        /// </summary>
        public virtual Int32 AccessFailedCount { get; set; }

        /// <summary>
        /// Used to define user status.
        /// True is disabled, default is false
        /// </summary>
        public virtual Boolean IsDisabled { get; set; }
        #endregion
    }
}
