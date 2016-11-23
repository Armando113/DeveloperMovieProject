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
        public AppSignInManager(AppUserManager _appUsrManager, IAuthenticationManager _authenticationManager) : base(_appUsrManager, _authenticationManager)
        {

        }

        public static AppSignInManager Create(IdentityFactoryOptions<AppSignInManager> _options, IOwinContext _context)
        {
            return new Models.AppSignInManager(_context.GetUserManager<AppUserManager>(), _context.Authentication);
        }
    }
}