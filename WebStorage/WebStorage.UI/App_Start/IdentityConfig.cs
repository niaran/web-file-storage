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
using KatanaContrib.Security.VK;

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

            //https://developers.facebook.com/apps/
            //https://localhost:44349/signin-facebook
            app.UseFacebookAuthentication("195976144194610","7c91f8bfd89f0b1bf3e0cf2e0dfa69fb");

            //https://github.com/settings/developers
            //https://localhost:44349/signin-github
            app.UseGithubAuthentication("8ab02bf3204e39faacdc", "e96c920a1130d8d9b9708e4d8adf06ca0898ab13");

            //https://apps.dev.microsoft.com/#/appList
            //https://localhost:44349/signin-microsoft
            app.UseMicrosoftAccountAuthentication("59359d74-e18f-4a5c-b21a-6f4d7753d392", "rB0ZE3CcxUyYosUrtPYFUTH");

            //https://vk.com/apps?act=manage
            //https://localhost:44349/signin-vk
            app.UseVkontakteAuthentication("5775688", "RY21KkaQXtgWJPcTjPJ9");
        }
    }   
}