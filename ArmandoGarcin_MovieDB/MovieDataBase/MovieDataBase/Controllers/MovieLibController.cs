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
            //Get the list from the Database


            return View(MovieData.MovieDBManager.GetMovieList());
        }

        [HttpGet]
        public ActionResult Edit(int id, string _name, string _genre, int _year)
        {
            //Add the movie
            MovieData.MovieDBManager.AddMovie(id, _name, _genre, _year);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Edit(string _msg)
        {
            //Update movies

            Console.WriteLine(_msg);

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