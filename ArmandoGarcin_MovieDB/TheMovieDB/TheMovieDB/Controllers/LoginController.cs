using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TheMovieDB.MovieData;

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
        public ActionResult CreateAccount()
        {
            return View();
        }

        //The HttpPost action function for creating a user
        [HttpPost]
        public ActionResult CreateAccount(User usr)
        {
            Console.WriteLine("Hi there!");

            //Get the values from the object model
            MovieDBContext DBContext = new MovieDBContext();

            //Add ID here
            usr.ID = 1;

            //Insert the new user to the Database
            DBContext.Users.Add(usr);

            //Save changes
            DBContext.SaveChanges();

            return View();
        }

        public ActionResult ViewAccounts()
        {
            //Get the values from the object model
            MovieDBContext DBContext = new MovieDBContext();

            List<User> UserList = new List<MovieData.User>();

            //Get the users from the database
            foreach(User _user in DBContext.Users)
            {
                UserList.Add(_user);
            }

            return View(UserList);
        }
    }
}