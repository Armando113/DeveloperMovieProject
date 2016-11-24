using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TheMovieDB.MovieData;
using TheMovieDB.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using System.Net;
using Microsoft.Owin.Security;

namespace TheMovieDB.Controllers
{
    public class LoginController : Controller
    {

        private AppSignInManager privSignInMan;
        private AppUserManager privUserMan;

        public AppSignInManager signInManager
        {
            get
            {
                return privSignInMan ?? HttpContext.GetOwinContext().Get<AppSignInManager>();
            }

            set
            {
                privSignInMan = value;
            }
        }

        public AppUserManager appUserMan
        {
            get
            {
                return privUserMan ?? HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }

            set
            {
                privUserMan = value;
            }
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        //The entry point of the page
        [HttpGet]
        [AllowAnonymous]
        public ActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateUser(RegisterViewModel _model)
        {
            if(ModelState.IsValid)
            {
                var user = new AppUser { UserName = _model.UserName, Email = _model.Email };
                var result = await appUserMan.CreateAsync(user, _model.Password);

                if(result.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            return View(_model);
        }

        public ActionResult ViewAccounts()
        {
            //Get the values from the object model
            IdentityDBContext DBContext = new IdentityDBContext();

            List<Models.AppUser> UserList = new List<Models.AppUser>();

            //Get the users from the database
            foreach(Models.AppUser _user in DBContext.Users)
            {
                UserList.Add(_user);
            }

            return View(UserList);
        }

        //HttpGet
        [AllowAnonymous]
        public ActionResult LoginUser(string _returnURL)
        {
            ViewBag.ReturnURL = _returnURL;
            return View();
        }

        //HttpPost
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LoginUser(LoginViewModel _model, string _returnURL)
        {
            if(!ModelState.IsValid)
            {
                return View(_model);
            }

            //PasswordSignInAsync works with username, NOT Email!!
            var result = await signInManager.PasswordSignInAsync(_model.UserName, _model.Password, _model.RememberMe, shouldLockout: false);

            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(_returnURL);
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt");
                    return View(_model);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        private void AddErrors(IdentityResult _result)
        {
            foreach(var error in _result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string _returnURL)
        {
            if(Url.IsLocalUrl(_returnURL))
            {
                return Redirect(_returnURL);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}