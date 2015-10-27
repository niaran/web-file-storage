using System;

namespace WebStorage.Domain.Entities
{
    public class IdentityUserLogin<TKey> where TKey : class
    {
        /// <summary>
        /// Gets or sets the login provider 
        /// for the login
        /// </summary>
        public virtual String LoginProvider { get; set; }

        /// <summary>
        /// Gets or sets the key representing 
        /// the login for the provider
        /// </summary>
        public virtual string ProviderKey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual TKey UserId { get; set; }
    }
}
