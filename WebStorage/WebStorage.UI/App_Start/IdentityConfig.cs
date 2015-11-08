using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Twitter;
using Owin;
using WebStorage.Domain.Concrete;
using WebStorage.Domain.Entities;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Security;
using KatanaContrib.Security.Github;
using KatanaContrib.Security.LinkedIn;
using Facebook;
using Microsoft.Owin.Security.Facebook;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Security.Claims;

namespace WebStorage.UI
{
    
    public class IdentityConfig
    {
        public void Configuration(IAppBuilder app)
        {
            app.CreatePerOwinContext<AppDbContext>(AppDbContext.Create);
            app.CreatePerOwinContext<AppUserManager>(AppUserManager.Create);
            app.CreatePerOwinContext<AppSignInManager>(AppSignInManager.Create);

            app.UseCookieAuthentication(
                new CookieAuthenticationOptions()
                {
                    //AuthenticationMode = AuthenticationMode.Active,
                    AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                    LoginPath = new PathString("/Account/Login")
                }
                );

            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Twitter
            
            app.UseTwitterAuthentication(new TwitterAuthenticationOptions
            {
                ConsumerKey = "ZEhQno1BxtZXfCyod0ccIxQfT",
                ConsumerSecret = "DDXJHOIQs2BPiYVecgi6Sx2VSAdsgnoLWveXKOiii6enuvjeOm",
                BackchannelCertificateValidator = new CertificateSubjectKeyIdentifierValidator(
                new[]
                {
                    "A5EF0B11CEC04103A34A659048B21CE0572D7D47", 
                    "0D445C165344C1827E1D20AB25F40163D8BE79A5", 
                    "7FD365A7C2DDECBBF03009F34339FA02AF333133", 
                    "39A55D933676616E73A761DFA16A7E59CDE66FAD", 
                    "5168FF90AF0207753CCCD9656462A212B859723B",
                    "B13EC36903F8BF4701D498261A0802EF63642BC3"
                })
            });

            // Facebook
            app.UseFacebookAuthentication(new FacebookAuthenticationOptions
            {
                AppId = "917487295003220",
                AppSecret = "bc8711e7f19d4607a2b67e45bde83f11"/*,
                Scope = { "email" },
                Provider = new FacebookAuthenticationProvider
                {
                    OnAuthenticated = context =>
                    {
                        context.Identity.AddClaim(new System.Security.Claims.Claim("FacebookAccessToken", context.AccessToken));
                        return Task.FromResult(true);
                    }
                }*/
            });

            // GitHub
            app.UseGithubAuthentication("58f46fe092484b58a158", "e069bfbe0b498b8258951847573258579ce92203");

            // LinkedIn
            app.UseLinkedInAuthentication("77fr5gyx4dgr54", "IN3HJqpSRuFfVpAe");
        }
    }   
}