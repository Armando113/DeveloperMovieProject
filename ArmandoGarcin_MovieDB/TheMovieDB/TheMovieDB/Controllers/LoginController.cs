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
    [Authorize]
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
        public async Task<ActionResult> CreateUser(RegisterViewModel model)
        {
            if(ModelState.IsValid)
            {
                var user = new AppUser { UserName = model.UserName, Email = model.Email, Phone = model.Phone };
                var result = await appUserMan.CreateAsync(user, model.Password);

                //If we succesfully created and there is no one signed in, sign in the new user
                if(result.Succeeded && !AuthenticationManager.User.Identity.IsAuthenticated)
                {
                    await signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    return RedirectToAction("Index", "Login");
                }
                //If we succesfully created the user, but we are already signed in
                else if(result.Succeeded && AuthenticationManager.User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("ViewAccounts", "Login");
                }
                //Add errors if there are any
                AddErrors(result);
            }
            //Return the view with the Model
            return View(model);
        }

        public ActionResult ViewAccounts()
        {
            //Get the values from the object model
            IdentityDBContext DBContext = new IdentityDBContext();

            List<Models.AppUser> UserList = DBContext.Users.ToList();

            return View(UserList);
        }

        //HttpGet
        [AllowAnonymous]
        public ActionResult LoginUser(string ReturnURL)
        {
            ViewBag.ReturnURL = ReturnURL;
            return View();
        }

        //HttpPost
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LoginUser(LoginViewModel model, string ReturnURL)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            //PasswordSignInAsync works with username, NOT Email!!
            var result = await signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);

            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(ReturnURL);
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt");
                    return View(model);
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
        [Authorize]
        public ActionResult EditUser(string UserID)
        {
            if(Request.IsAuthenticated)
            {
                //Find the user if it exists
                IdentityDBContext dbContext = new IdentityDBContext();
                //Use entity to find the user found
                AppUser UserFound = dbContext.Users.Find(UserID);

                if(UserFound == null)
                {
                    return View();
                }

                var tEdit = new EditViewModel { UserName = UserFound.UserName, Email = UserFound.Email, Phone = UserFound.Phone };

                return View(tEdit);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> EditUser(EditViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return RedirectToAction("Index", "Login");
            }

            //Update data
            var tUser = appUserMan.FindByEmail(model.Email);

            IdentityDBContext dbContext = new IdentityDBContext();

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                //Edit all available settings
                tUser.Phone = model.Phone;

                //Update in the database
                await appUserMan.UpdateAsync(tUser);
                transaction.Commit();
            }

            return RedirectToAction("ViewAccounts", "Login");
        }

        [HttpGet]
        public ActionResult DeleteUser(string UserID)
        {
            if(Request.IsAuthenticated)
            {
                if (!User.Identity.GetUserId().Equals(UserID))
                {
                    //Find the user if it exists
                    IdentityDBContext dbContext = new IdentityDBContext();

                    AppUser UserFound = dbContext.Users.Find(UserID);

                    var tDelete = new DeleteViewModel { UserName = UserFound.UserName, Email = UserFound.Email };

                    return View(tDelete);
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