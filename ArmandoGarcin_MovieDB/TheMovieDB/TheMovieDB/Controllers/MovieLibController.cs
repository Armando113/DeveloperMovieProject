using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TheMovieDB.Controllers
{
    public class MovieLibController : Controller
    {
        // GET: MovieLib
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MovieView()
        {
            return View();
        }
    }
}