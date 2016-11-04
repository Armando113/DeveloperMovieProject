using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MovieDataBase.Controllers
{
    public class MovieLibController : Controller
    {

        public MovieLibController()
        {

        }

        [ActionName("Find")]
        public ActionResult GetById(int _id)
        {

            return View();
        }

        // GET: MovieLib
        public ActionResult Index()
        {
            List<MovieData.Movie> movieList = new List<MovieData.Movie>();

            movieList.Add(new MovieData.Movie("LOTR", "Action-Fantasy", 2003));
            movieList.Add(new MovieData.Movie("Star Wars 4", "Sci-Fi", 1978));
            movieList.Add(new MovieData.Movie("Revenant", "Drama", 2015));

            return View(movieList);
        }

        [HttpPost]
        public ActionResult Edit(string _mssg)
        {
            //Update movies

            Console.WriteLine(_mssg);

            return RedirectToAction("Index");
        }

        [HttpDelete]
        public ActionResult Delete(int _id)
        {
            //Delete

            return RedirectToAction("Index");
        }
    }
}