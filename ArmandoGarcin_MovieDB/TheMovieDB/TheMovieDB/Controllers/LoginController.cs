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

        private AppSignInManager _SignInMan;
        private AppUserManager _UserMan;

        public AppSignInManager signInManager
        {
            get
            {
                return _SignInMan ?? HttpContext.GetOwinContext().Get<AppSignInManager>();
            }

            set
            {
                _SignInMan = value;
            }
        }

        public AppUserManager appUserManager
        {
            get
            {
                return _UserMan ?? HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }

            set
            {
                _UserMan = value;
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
                var user = new AppUser { UserName = model.userName, Email = model.email, Phone = model.phone };
                var result = await appUserManager.CreateAsync(user, model.Password);

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

            return View(model);
        }

        public ActionResult ViewAccounts()
        {
            //Get the values from the object model
            IdentityDBContext dbContext = new IdentityDBContext();
            //Create a string woith the current user
            string currentUserId = User.Identity.GetUserId();
            //Create the list of users, and exclude the one currently logged in
            List<AppUser> UserList = dbContext.Users.Select(x => x).Where(x => !x.Id.Equals(currentUserId)).ToList();
            //Return the list to the View
            return View(UserList);
        }

        //HttpGet
        [AllowAnonymous]
        public ActionResult LoginUser(string returnURL)
        {
            ViewBag.ReturnURL = returnURL;
            return View();
        }

        //HttpPost
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LoginUser(LoginViewModel model, string returnURL)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }
            //PasswordSignInAsync works with username, NOT Email!!
            var result = await signInManager.PasswordSignInAsync(model.userName, model.password, model.rememberMe, shouldLockout: false);
            //Check the result of the SignIn
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnURL);
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
        public ActionResult EditUser(string user)
        {
            if(!Request.IsAuthenticated)
            {
                return View();
            }
            //Find the user if it exists
            IdentityDBContext dbContext = new IdentityDBContext();
            //Use Entity to get the selected user
            var foundUser = dbContext.Users.Select(x => x).FirstOrDefault(x => x.Id.Equals(user));
            //Create the model
            var editModel = new EditViewModel { userName = foundUser.UserName, email = foundUser.Email, phone = foundUser.Phone };
            //Return the edit model
            return View(editModel);
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
            var userToEdit = appUserManager.FindByEmail(model.email);

            IdentityDBContext dbContext = new IdentityDBContext();

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                //Edit all available settings
                userToEdit.Phone = model.phone;

                //Update in the database
                await appUserManager.UpdateAsync(userToEdit);
                transaction.Commit();
            }

            return RedirectToAction("ViewAccounts", "Login");
        }

        [HttpGet]
        public ActionResult DeleteUser(string user)
        {
            //If we are not logged in...
            if(!Request.IsAuthenticated)
            {
                return View();
            }
            //Check that we are not deleting the current user
            if (!User.Identity.GetUserName().Equals(user))
            {
                return View();
            }
            //Create the context
            IdentityDBContext dbContext = new IdentityDBContext();
            //Find the desired user to delete
            var userToDelete = dbContext.Users.FirstOrDefault(x => x.Id.Equals(user));
            //Create the model for the view
            var deleteModel = new DeleteViewModel { userName = userToDelete.UserName, email = userToDelete.Email };

            return View(deleteModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteUser(DeleteViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return RedirectToAction("Index", "Login");
            }
            //Get the user to delete
            var deleteUser = appUserManager.FindByEmail(model.email);
            //Get the Logins from the User
            var userLogins = deleteUser.Logins;
            //Get the roles the user has
            var userRoles = await appUserManager.GetRolesAsync(deleteUser.Id);
            //create the DB context
            IdentityDBContext dbContext = new IdentityDBContext();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                foreach(var logins in userLogins.ToList())
                {
                    await appUserManager.RemoveLoginAsync(logins.UserId, new UserLoginInfo(logins.LoginProvider, logins.ProviderKey));
                }

                if(userRoles.Count() > 0)
                {
                    foreach(var item in userRoles.ToList())
                    {
                        var result = await appUserManager.RemoveFromRoleAsync(deleteUser.Id, item);
                    }
                }

                await appUserManager.DeleteAsync(deleteUser);
                transaction.Commit();
            }
            //Else, just return to the View Accounts page
            return RedirectToAction("ViewAccounts", "Login");
        }

        private void AddErrors(IdentityResult result)
        {
            foreach(var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnURL)
        {
            if(Url.IsLocalUrl(returnURL))
            {
                return Redirect(returnURL);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}