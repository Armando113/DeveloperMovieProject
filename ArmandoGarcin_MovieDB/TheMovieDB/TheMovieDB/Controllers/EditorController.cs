using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TheMovieDB.Controllers
{
    public class EditorController : Controller
    {
        // GET: Ediitor
        public ActionResult Index()
        {
            return View();
        }
    }
}