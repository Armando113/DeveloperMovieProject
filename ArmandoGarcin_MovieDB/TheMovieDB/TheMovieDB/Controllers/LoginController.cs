using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TheMovieDB.MovieData;
using TheMovieDB.Models;
using Microsoft.AspNet.Identity;

namespace TheMovieDB.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        //The entry point of the page
        public ActionResult CreateUser()
        {
            return View();
        }

        //The HttpPost action function for creating a user
        [HttpPost]
        public ActionResult CreateUser(AppUser usr)
        {
            Console.WriteLine("Hi there!");

            ////Get the values from the object model
            //MovieDBContext DBContext = new MovieDBContext();
            ////Add ID here
            //usr.ID = 1;
            ////Insert the new user to the Database
            //DBContext.Users.Add(usr);
            ////Save changes
            //DBContext.SaveChanges();

            //Using Identity
            AppUser user = new AppUser();
            AppUserStore userStore = new AppUserStore(new IdentityDBContext());
            AppUserManager userMan = new AppUserManager(userStore);

            user.Email = usr.Email;
            user.UserName = usr.UserName;
            user.PasswordHash = usr.PasswordHash;

            IdentityResult result = userMan.Create(user);
            if(!result.Succeeded)
            {
                Console.WriteLine("Error creating User");
            }

            return View();
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
    }
}