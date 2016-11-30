using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TheMovieDB.MovieData;
using TheMovieDB.Models;

namespace TheMovieDB.Controllers
{
    public class MovieLibController : Controller
    {
        // GET: MovieLib
        public ActionResult Index()
        {
            IdentityDBContext db = new IdentityDBContext();

            List<Genre> genreList = new List<Genre>();

            foreach(Genre genre in db.Genres)
            {
                genreList.Add(genre);
            }

            return View(genreList);
        }

        public ActionResult MovieView()
        {
            return View();
        }

        /*-----Add Genre to the library-----*/

        public ActionResult AddGenre()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddGenre(AddGenreModel _model)
        {
            int tID = GetGenreID(_model.GenreName);

            IdentityDBContext db = new IdentityDBContext();

            foreach(Genre genre in db.Genres)
            {
                if(genre.GenreID == tID)
                {
                    //Add Error message here
                    return View();
                }
            }

            //Genre does not exist, proceed with the addition to DB
            Genre nuGenre = new Genre() { GenreID = tID, GenreName = _model.GenreName};

            //Add it to the DB
            db.Genres.Add(nuGenre);
            //Commit
            db.SaveChanges();

            return RedirectToAction("Index", "MovieLib");
        }

        [HttpGet]
        public ActionResult DeleteGenre(int _genreID)
        {
            if(Request.IsAuthenticated)
            {
                IdentityDBContext db = new IdentityDBContext();

                foreach(Genre genre in db.Genres)
                {
                    if(genre.GenreID == _genreID)
                    {
                        //Delete the Genre
                        db.Genres.Remove(genre);
                        break;
                    }
                    //else if(genre.GenreID == _genreID && genre.Movies.Count > 0)
                    //{
                    //    //add Errors here
                    //    break;
                    //}
                }

                //save changes
                db.SaveChanges();
            }
            return RedirectToAction("Index", "MovieLib");
        }

        private int GetGenreID(string _genre)
        {
            //Get the hash code of the 
            int nameHash = 128;
            string lowStr = _genre.ToLower();

            foreach(char c in lowStr)
            {
                nameHash += c;
            }

            return nameHash;
        }

        private int GetMovieID(string _name)
        {
            //Get the hash code of the 
            int nameHash = 16;
            string lowStr = _name.ToLower();

            foreach (char c in lowStr)
            {
                nameHash += c;
            }

            return nameHash;
        }
    }
}