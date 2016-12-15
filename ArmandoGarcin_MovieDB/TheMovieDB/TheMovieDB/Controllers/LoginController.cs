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
                var user = new AppUser { UserName = _model.UserName, Email = _model.Email, Phone = _model.Phone };
                var result = await appUserMan.CreateAsync(user, _model.Password);

                if(result.Succeeded && !AuthenticationManager.User.Identity.IsAuthenticated)
                {
                    await signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    return RedirectToAction("Index", "Login");
                }else if(result.Succeeded && AuthenticationManager.User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("ViewAccounts", "Login");
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

            //Get our current logged in user
            string userID = User.Identity.GetUserName();

            //Get the users from the database
            foreach(Models.AppUser _user in DBContext.Users)
            {
                if(!_user.UserName.Equals(userID))
                {
                    UserList.Add(_user);
                }
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

        [HttpGet]
        public ActionResult EditUser(string _user)
        {
            if(Request.IsAuthenticated)
            {
                //Find the user if it exists
                IdentityDBContext dbContext = new IdentityDBContext();

                foreach (AppUser tUser in dbContext.Users)
                {
                    if (tUser.UserName.Equals(_user))
                    {
                        var tEdit = new EditViewModel { UserName = tUser.UserName, Email = tUser.Email, Phone = tUser.Phone };

                        return View(tEdit);
                    }
                }
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> EditUser(EditViewModel _model)
        {
            if(!ModelState.IsValid)
            {
                return RedirectToAction("Index", "Login");
            }

            //Update data
            var tUser = appUserMan.FindByEmail(_model.Email);

            IdentityDBContext dbContext = new IdentityDBContext();

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                //Edit all available settings
                tUser.Phone = _model.Phone;

                //Update in the database
                await appUserMan.UpdateAsync(tUser);
                transaction.Commit();
            }

            return RedirectToAction("ViewAccounts", "Login");
        }

        [HttpGet]
        public ActionResult DeleteUser(string _user)
        {
            if(Request.IsAuthenticated)
            {
                if (!User.Identity.GetUserName().Equals(_user))
                {
                    //Find the user if it exists
                    IdentityDBContext dbContext = new IdentityDBContext();

                    foreach (AppUser tUser in dbContext.Users)
                    {
                        if (tUser.UserName.Equals(_user))
                        {
                            var tDelete = new DeleteViewModel { UserName = tUser.UserName, Email = tUser.Email };

                            return View(tDelete);
                        }
                    }
                }
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteUser(DeleteViewModel _model)
        {
            if(!ModelState.IsValid)
            {
                return RedirectToAction("Index", "Login");
            }

            var tUser = appUserMan.FindByEmail(_model.Email);
            var tLogins = tUser.Logins;
            var tRoles = await appUserMan.GetRolesAsync(tUser.Id);
            IdentityDBContext dbContext = new IdentityDBContext();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                foreach(var logins in tLogins.ToList())
                {
                    await appUserMan.RemoveLoginAsync(logins.UserId, new UserLoginInfo(logins.LoginProvider, logins.ProviderKey));
                }

                if(tRoles.Count() > 0)
                {
                    foreach(var item in tRoles.ToList())
                    {
                        var result = await appUserMan.RemoveFromRoleAsync(tUser.Id, item);
                    }
                }

                await appUserMan.DeleteAsync(tUser);
                transaction.Commit();
            }
            //Else, just return to the View Accounts page
            return RedirectToAction("ViewAccounts", "Login");
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

        private AppUser FindUserViaUsername(string _username)
        {

            //Find the user if it exists
            IdentityDBContext dbContext = new IdentityDBContext();


            foreach (AppUser tUser in dbContext.Users)
            {
                if (tUser.UserName.Equals(_username))
                {
                    return tUser;
                }
            }

            return null;
        }
    }
}