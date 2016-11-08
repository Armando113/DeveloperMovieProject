using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieDataBase.MovieData
{
    public class MovieDBManager
    {
        //This class will help us interact with the Database
        //where the movies are being stored

        private static MovieDBManager pInstance;

        private MovieDBEntities1 movieDB;

        public static bool bTestBool = false;

        private List<Movie> movieList;
        private MovieDBManager()
        {
            movieList = new List<Movie>();

            //Create the DB class
            movieDB = new MovieDBEntities1();

            bTestBool = true;

            AddFromDB();

        }

        private Movy CreateMovy(int _id, string _name, string _genre, int _year)
        {
            return new Movy(_id, _name, _genre, _year);
        }

        private void AddFromDB()
        {
            foreach(Movy movy in movieDB.Movies)
            {
                movieList.Add(new Movie(movy.Name, movy.Genre, (int)movy.Year));
            }
        }

        private void AddMovieToDB(int _id, string _name, string _genre, int _year)
        {
            Movy movy = new Movy(_id, _name, _genre, _year);

            //Add it to the Database
            movieDB.Movies.Add(movy);

            //Don't forget to save changes
            movieDB.SaveChanges();
        }

        private static MovieDBManager GetInstance()
        {
            if(pInstance == null)
            {
                pInstance = new MovieDBManager();
            }
            return pInstance;
        }

        public static List<Movie> GetMovieList()
        {
            return GetInstance().movieList;
        }

        public static void AddMovie(int _id, string _name, string _genre, int _year)
        {
            GetInstance().movieList.Add(new Movie(_name, _genre, _year));

            GetInstance().AddMovieToDB(_id, _name, _genre, _year);
        }
    }
}