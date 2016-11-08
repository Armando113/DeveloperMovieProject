using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MovieDataBase.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            if(Login.LoginManager.IsLoggedIn())
            {
                return RedirectToAction("Account");
            }
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult ProfileView()
        {
            return View();
        }
    }
}