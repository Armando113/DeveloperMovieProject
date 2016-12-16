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
        public ActionResult AddGenre(AddGenreModel model)
        {
            if(ModelState.IsValid)
            {
                int temporalID = GetGenreID(model.genreName);

                IdentityDBContext db = new IdentityDBContext();

                foreach (Genre genre in db.Genres)
                {
                    if (genre.genreID == temporalID)
                    {
                        //Add Error message here
                        return View();
                    }
                }

                //Genre does not exist, proceed with the addition to DB
                Genre newGenre = new Genre() { genreID = temporalID, genreName = model.genreName };

                //Add it to the DB
                db.Genres.Add(newGenre);

                //Commit
                db.SaveChanges();

                return RedirectToAction("Index", "MovieLib");
            }

            return RedirectToAction("AddGenre", "MovieLib");
        }

        [HttpGet]
        public ActionResult DeleteGenre(int genreID)
        {
            if(Request.IsAuthenticated)
            {
                IdentityDBContext db = new IdentityDBContext();

                List<Movie> movieList = GetMovieList(genreID, db);

                foreach(Genre genre in db.Genres)
                {
                    if(genre.genreID == genreID && movieList.Count == 0)
                    {
                        //Delete the Genre
                        db.Genres.Remove(genre);
                        break;
                    }
                    else if (genre.genreID == genreID && movieList.Count > 0)
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
        public ActionResult ViewMovies(int genreID)
        {
            IdentityDBContext dbContext = new IdentityDBContext();

            ViewMovieModel model = new ViewMovieModel() { movieGenreID = genreID, movieList = GetMovieList(genreID, dbContext) };

            return View(model);
        }

        [HttpGet]
        public ActionResult AddMovie(int genreID)
        {
            if(Request.IsAuthenticated)
            {
                IdentityDBContext db = new IdentityDBContext();

                Genre foundGenre = db.Genres.Find(genreID);

                if(foundGenre != null)
                {
                    AddMovieModel newModel = new AddMovieModel() { movieGenreID = foundGenre.genreID, movieGenreName = foundGenre.genreName };

                    return View(newModel);
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
                //Find the Genre by ID in the DB
                Genre foundGenre = db.Genres.Find(model.movieGenreID);
                //Create the movie 
                Movie newMovie = new Movie() { movieID = GetMovieID(model.movieName), movieName = model.movieName, releaseDate = model.releaseDate, movieGenre = foundGenre, movieGenreID = foundGenre.genreID };
                //Get the list from the Genre table
                List<Movie> genreContext = GetMovieList(model.movieGenreID, db);
                //Add movie to the list
                genreContext.Add(newMovie);
                //Now, add the Movie to the DB
                db.Movies.Add(newMovie);
                //Save changes in the DB
                db.SaveChanges();

                return RedirectToAction("ViewMovies", "MovieLib", new { genreID = model.movieGenreID });
            }

            return RedirectToAction("Index", "MovieLib");
        }

        [HttpGet]
        public ActionResult DeleteMovie(int movieID)
        {
            if(Request.IsAuthenticated)
            {
                //Get the Database
                IdentityDBContext db = new IdentityDBContext();
                //Find the movie
                Movie foundMovie = db.Movies.Find(movieID);
                //Check if the movie exists
                if(foundMovie != null)
                {
                    //Delete the movie from the Genre list
                    List<Movie> movieList = GetMovieList((int)foundMovie.movieGenreID, db);
                    //Romeove it from the list
                    movieList.Remove(foundMovie);
                    //Remove
                    db.Movies.Remove(foundMovie);
                    //Save Changes
                    db.SaveChanges();
                    //Return to the movie list of the genre
                    return RedirectToAction("ViewMovies", "MovieLib", new { genreID = foundMovie.movieGenreID });
                }
                return RedirectToAction("Index", "MovieLib");
            }
            return RedirectToAction("LoginUser", "Login");
        }

        [HttpGet]
        public ActionResult EditMovie(int movieID)
        {
            if(Request.IsAuthenticated)
            {
                IdentityDBContext db = new IdentityDBContext();

                Movie foundMovie = db.Movies.Find(movieID);

                EditMovieModel editModel = new EditMovieModel()
                {
                    movieGenreID = (int)foundMovie.movieGenreID,
                    movieGenreName = foundMovie.movieGenre.genreName,
                    movieID = foundMovie.movieID,
                    movieName = foundMovie.movieName,
                    releaseDate = (DateTime)foundMovie.releaseDate
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

                Movie foundMovie = db.Movies.Find(model.movieID);

                foundMovie.movieName = model.movieName;
                foundMovie.releaseDate = model.releaseDate;

                db.Movies.Attach(foundMovie);

                db.Entry(foundMovie).State = EntityState.Modified;

                //Save changes
                db.SaveChanges();

                return RedirectToAction("ViewMovies", "MovieLib", new { genreID = foundMovie.movieGenreID });
            }
            return RedirectToAction("Index", "MovieLib");
        }

        private int GetGenreID(string genre)
        {
            //Get the hash code of the 
            int nameHash = 128;
            string lowString = genre.ToLower();

            foreach(char c in lowString)
            {
                nameHash += c;
            }

            return nameHash;
        }

        private int GetMovieID(string name)
        {
            //Get the hash code of the 
            int nameHash = 16;
            string lowString = name.ToLower();

            foreach (char c in lowString)
            {
                nameHash += c;
            }

            return nameHash;
        }

        private List<Movie> GetMovieList(int genreID, IdentityDBContext db)
        {
            var genreModel = db.Genres.Include(m => m.movies).SingleOrDefault(m => m.genreID == genreID);

            return genreModel.movies.ToList();
        }
    }
}