using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TheMovieDB.MovieData
{
    public class Movie
    {
        //Database Columns
        //Our ID (Primary Key)
        public int MovieID { get; set; }
        //The name of the Movie
        public string MovieName { get; set; }
        //The Genre of the movie
        public virtual Genre MovieGenre { get; set; }
        //The Date of release
        public DateTime? ReleaseDate { get; set; }

        public Movie()
        {

        }
    }
}