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

            //https://developers.facebook.com/apps/
            //https://localhost:44349/signin-facebook
            app.UseFacebookAuthentication(new FacebookAuthenticationOptions
            {
                AppId = "195976144194610",
                AppSecret = "7c91f8bfd89f0b1bf3e0cf2e0dfa69fb"/*,
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

            //https://github.com/settings/developers
            //https://localhost:44349/signin-github
            app.UseGithubAuthentication("8ab02bf3204e39faacdc", "e96c920a1130d8d9b9708e4d8adf06ca0898ab13");

            //https://apps.dev.microsoft.com/#/appList
            //https://localhost:44349/signin-microsoft
            app.UseMicrosoftAccountAuthentication("59359d74-e18f-4a5c-b21a-6f4d7753d392", "rB0ZE3CcxUyYosUrtPYFUTH");

            // LinkedIn
            app.UseLinkedInAuthentication("77fr5gyx4dgr54", "IN3HJqpSRuFfVpAe");
        }
    }   
}