using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using WebStorage.Domain.Concrete;

namespace WebStorage.Domain.Entities
{
    public class AppUserManager : UserManager<AppUser>
    {
        public AppUserManager(IUserStore<AppUser> store) : base(store)
        { }

        public static AppUserManager Create(IdentityFactoryOptions<AppUserManager> options, IOwinContext context)
        {
            AppDbContext db = context.Get<AppDbContext>();
            AppUserManager manager = new AppUserManager(new UserStore<AppUser>(db));

            /*A password policy is defined by creating an instance of the PasswordValidatorclass, 
            setting the property values, and using the object as the value for the PasswordValidator property 
            in the Createmethod that OWIN uses to instantiate the AppUserManagerclass*/
            manager.PasswordValidator = new PasswordValidator()
            {
                RequireDigit = false,
                RequiredLength = 6,
                RequireLowercase = true,
                RequireUppercase = true,
                RequireNonLetterOrDigit = false
            };

            /* More general validation can be performed by creating an instance of the 
            UserValidatorclass and using the 
            properties it defines to restrict other user property values*/
            manager.UserValidator = new UserValidator<AppUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = true,
                RequireUniqueEmail = true
            };

            return manager;
        }
    }
}
