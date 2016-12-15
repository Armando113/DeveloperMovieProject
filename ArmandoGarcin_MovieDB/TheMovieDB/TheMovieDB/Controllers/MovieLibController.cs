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
            if(ModelState.IsValid)
            {
                int tID = GetGenreID(_model.GenreName);

                IdentityDBContext db = new IdentityDBContext();

                foreach (Genre genre in db.Genres)
                {
                    if (genre.GenreID == tID)
                    {
                        //Add Error message here
                        return View();
                    }
                }

                //Genre does not exist, proceed with the addition to DB
                Genre nuGenre = new Genre() { GenreID = tID, GenreName = _model.GenreName };

                //Add it to the DB
                db.Genres.Add(nuGenre);

                //Commit
                db.SaveChanges();

                return RedirectToAction("Index", "MovieLib");
            }

            return RedirectToAction("AddGenre", "MovieLib");
        }

        [HttpGet]
        public ActionResult DeleteGenre(int _genreID)
        {
            if(Request.IsAuthenticated)
            {
                IdentityDBContext db = new IdentityDBContext();

                List<Movie> movieList = GetMovieList(_genreID, db);

                foreach(Genre genre in db.Genres)
                {
                    if(genre.GenreID == _genreID && movieList.Count == 0)
                    {
                        //Delete the Genre
                        db.Genres.Remove(genre);
                        break;
                    }
                    else if (genre.GenreID == _genreID && movieList.Count > 0)
                    {
                        //add Errors here
                        Console.WriteLine("Error when deleting Genre");
                        break;
                    }
                }
                //save changes
                db.SaveChanges();
                return RedirectToAction("Index", "MovieLib");
            }
            return RedirectToAction("LoginUser", "Login");
        }

        [HttpGet]
        public ActionResult ViewMovies(int _genreID)
        {
            IdentityDBContext dbContext = new IdentityDBContext();

            ViewMovieModel model = new ViewMovieModel() { MovieGenreID = _genreID, MovieList = GetMovieList(_genreID, dbContext) };

            return View(model);
        }

        [HttpGet]
        public ActionResult AddMovie(int _genreID)
        {
            if(Request.IsAuthenticated)
            {
                IdentityDBContext db = new IdentityDBContext();

                Genre tGenre = db.Genres.Find(_genreID);

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
        public ActionResult AddMovie(AddMovieModel _model)
        {
            if(ModelState.IsValid)
            {
                IdentityDBContext db = new IdentityDBContext();

                Genre tGenre = db.Genres.Find(_model.MovieGenreID);

                Movie tMovie = new Movie() { MovieID = GetMovieID(_model.MovieName), MovieName = _model.MovieName, ReleaseDate = _model.ReleaseDate, MovieGenre = tGenre, MovieGenreID = tGenre.GenreID };

                //Add it to the Genre list
                var genreContext = db.Genres.Include(m => m.Movies).SingleOrDefault(m => m.GenreID == _model.MovieGenreID);
                genreContext.Movies.ToList().Add(tMovie);

                //Now, add the Movie to the DB
                db.Movies.Add(tMovie);

                //Save changes in the DB
                db.SaveChanges();

                return RedirectToAction("ViewMovies", "MovieLib", new { _genreID = _model.MovieGenreID });
            }

            return RedirectToAction("Index", "MovieLib");
        }

        [HttpGet]
        public ActionResult DeleteMovie(int _movieID)
        {
            if(Request.IsAuthenticated)
            {
                //Get the Database
                IdentityDBContext db = new IdentityDBContext();
                //Find the movie
                Movie tMovie = db.Movies.Find(_movieID);

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

                    return RedirectToAction("ViewMovies", "MovieLib", new { _genreID = tMovie.MovieGenreID });
                }

                return RedirectToAction("Index", "MovieLib");
            }
            return RedirectToAction("LoginUser", "Login");
        }

        [HttpGet]
        public ActionResult EditMovie(int _movieID)
        {
            if(Request.IsAuthenticated)
            {
                IdentityDBContext db = new IdentityDBContext();

                Movie tMovie = db.Movies.Find(_movieID);

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
        public ActionResult EditMovie(EditMovieModel _model)
        {
            if(ModelState.IsValid)
            {
                IdentityDBContext db = new IdentityDBContext();

                Movie tMovie = db.Movies.Find(_model.MovieID);

                tMovie.MovieName = _model.MovieName;
                tMovie.ReleaseDate = _model.ReleaseDate;

                db.Movies.Attach(tMovie);

                db.Entry(tMovie).State = EntityState.Modified;

                //Save changes
                db.SaveChanges();

                return RedirectToAction("ViewMovies", "MovieLib", new { _genreID = tMovie.MovieGenreID });
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

        private List<Movie> GetMovieList(int _genreID, IdentityDBContext _db)
        {
            var genreModel = _db.Genres.Include(m => m.Movies).SingleOrDefault(m => m.GenreID == _genreID);

            return genreModel.Movies.ToList();
        }
    }
}