using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieDataBase.MovieData
{
    public class Movie
    {
        //If we want to access variables, we need to make them public
        public string name;
        public string genre;
        public int year;

        public Movie(string _name, string _genre, int _year)
        {
            name = _name;
            genre = _genre;
            year = _year;
        }
    }
}