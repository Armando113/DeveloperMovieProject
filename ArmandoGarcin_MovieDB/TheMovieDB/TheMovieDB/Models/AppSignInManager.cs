using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin;

namespace TheMovieDB.Models
{
    public class AppSignInManager : SignInManager<AppUser, string>
    {

        //Constructor
        public AppSignInManager(AppUserManager appUsrManager, IAuthenticationManager authenticationManager) : base(appUsrManager, authenticationManager)
        {

        }

        public static AppSignInManager Create(IdentityFactoryOptions<AppSignInManager> options, IOwinContext context)
        {
            return new Models.AppSignInManager(context.GetUserManager<AppUserManager>(), context.Authentication);
        }
    }
}