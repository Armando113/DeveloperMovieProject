using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
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
            //Create the Context
            IdentityDBContext db = new IdentityDBContext();
            //harness the power of entity, and turn the Table into a list
            List<Genre> genreList = db.Genres.ToList();
            //Send the list to the view
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
        public ActionResult AddGenre(AddGenreModel model)
        {
            if(ModelState.IsValid)
            {
                //This stays
                IdentityDBContext db = new IdentityDBContext();

                //If we found at least one column, exit!
                if(db.Genres.Select(x => x.GenreName).Where(x => x.Equals(model.GenreName)).ToList().Count > 0)
                {
                    //Add errors here
                    return View();
                }
                //Genre does not exist, proceed with the addition to DB
                Genre nuGenre = new Genre() { GenreID = GetGenreID(model.GenreName), GenreName = model.GenreName };
                //Add it to the DB
                db.Genres.Add(nuGenre);
                //Commit
                db.SaveChanges();

                return RedirectToAction("Index", "MovieLib");
            }
            return RedirectToAction("AddGenre", "MovieLib");
        }

        [HttpGet]
        public ActionResult DeleteGenre(int GenreID)
        {
            if(Request.IsAuthenticated)
            {
                IdentityDBContext db = new IdentityDBContext();
                //Get
                List<Movie> movieList = GetMovieList(GenreID, db);
                //Find the genre using entity
                Genre resultGenre = db.Genres.Find(GenreID);
                //Check if we can delete it
                if(movieList.Count == 0)
                {
                    //Delete the Genre
                    db.Genres.Remove(resultGenre);
                }
                else
                {
                    //Add errors here
                }
                //save changes
                db.SaveChanges();
                return RedirectToAction("Index", "MovieLib");
            }
            return RedirectToAction("LoginUser", "Login");
        }

        [HttpGet]
        public ActionResult ViewMovies(int GenreID)
        {
            IdentityDBContext dbContext = new IdentityDBContext();

            ViewMovieModel model = new ViewMovieModel() { MovieGenreID = GenreID, MovieList = GetMovieList(GenreID, dbContext) };

            return View(model);
        }

        [HttpGet]
        public ActionResult AddMovie(int GenreID)
        {
            if(Request.IsAuthenticated)
            {
                IdentityDBContext db = new IdentityDBContext();

                Genre tGenre = db.Genres.Find(GenreID);

                if(tGenre != null)
                {
                    AddMovieModel tModel = new AddMovieModel() { MovieGenreID = tGenre.GenreID, MovieGenreName = tGenre.GenreName };

                    return View(tModel);
                }

                return RedirectToAction("Index", "MovieLib");
            }
            return RedirectToAction("LoginUser", "Login");
        }

        [HttpPost]
        public ActionResult AddMovie(AddMovieModel model)
        {
            if(ModelState.IsValid)
            {
                IdentityDBContext db = new IdentityDBContext();
                //Find the corresponding Genre
                Genre tGenre = db.Genres.Find(model.MovieGenreID);
                //Create the movie from our model
                Movie tMovie = new Movie() { MovieID = GetMovieID(model.MovieName), MovieName = model.MovieName, ReleaseDate = model.ReleaseDate, MovieGenre = tGenre, MovieGenreID = tGenre.GenreID };

                //Add it to the Genre list
                var genreContext = db.Genres.Include(m => m.Movies).SingleOrDefault(m => m.GenreID == model.MovieGenreID);
                genreContext.Movies.ToList().Add(tMovie);
                //Now, add the Movie to the DB
                db.Movies.Add(tMovie);
                //Save changes in the DB
                db.SaveChanges();

                return RedirectToAction("ViewMovies", "MovieLib", new { GenreID = model.MovieGenreID });
            }

            return RedirectToAction("Index", "MovieLib");
        }

        [HttpGet]
        public ActionResult DeleteMovie(int MovieID)
        {
            if(Request.IsAuthenticated)
            {
                //Get the Database
                IdentityDBContext db = new IdentityDBContext();
                //Find the movie
                Movie tMovie = db.Movies.Find(MovieID);

                if(tMovie != null)
                {
                    //Delete the movie from the Genre list
                    List<Movie> movieList = GetMovieList((int)tMovie.MovieGenreID, db);
                    //Romeove it from the list
                    movieList.Remove(tMovie);
                    //Remove
                    db.Movies.Remove(tMovie);
                    //Save Changes
                    db.SaveChanges();

                    return RedirectToAction("ViewMovies", "MovieLib", new { GenreID = tMovie.MovieGenreID });
                }

                return RedirectToAction("Index", "MovieLib");
            }
            return RedirectToAction("LoginUser", "Login");
        }

        [HttpGet]
        public ActionResult EditMovie(int MovieID)
        {
            if(Request.IsAuthenticated)
            {
                IdentityDBContext db = new IdentityDBContext();

                Movie tMovie = db.Movies.Find(MovieID);

                EditMovieModel editModel = new EditMovieModel()
                {
                    MovieGenreID = (int)tMovie.MovieGenreID,
                    MovieGenreName = tMovie.MovieGenre.GenreName,
                    MovieID = tMovie.MovieID,
                    MovieName = tMovie.MovieName,
                    ReleaseDate = (DateTime)tMovie.ReleaseDate
                };

                return View(editModel);
            }
            return RedirectToAction("LoginUser", "Login");
        }

        [HttpPost]
        public ActionResult EditMovie(EditMovieModel model)
        {
            if(ModelState.IsValid)
            {
                IdentityDBContext db = new IdentityDBContext();
                //Find the movie
                Movie tMovie = db.Movies.Find(model.MovieID);
                //Set the model
                tMovie.MovieName = model.MovieName;
                tMovie.ReleaseDate = model.ReleaseDate;

                db.Movies.Attach(tMovie);

                db.Entry(tMovie).State = EntityState.Modified;

                //Save changes
                db.SaveChanges();

                return RedirectToAction("ViewMovies", "MovieLib", new { GenreID = tMovie.MovieGenreID });
            }
            return RedirectToAction("Index", "MovieLib");
        }

        private int GetGenreID(string genre)
        {
            //Get the hash code of the 
            int nameHash = 128;
            string lowStr = genre.ToLower();

            foreach(char c in lowStr)
            {
                nameHash += c;
            }

            return nameHash;
        }

        private int GetMovieID(string name)
        {
            //Get the hash code of the 
            int nameHash = 16;
            string lowStr = name.ToLower();

            foreach (char c in lowStr)
            {
                nameHash += c;
            }

            return nameHash;
        }

        private List<Movie> GetMovieList(int GenreID, IdentityDBContext db)
        {
            var genreModel = db.Genres.Include(m => m.Movies).SingleOrDefault(m => m.GenreID == GenreID);

            return genreModel.Movies.ToList();
        }
    }
}