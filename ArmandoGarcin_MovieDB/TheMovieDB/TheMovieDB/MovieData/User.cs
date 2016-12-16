using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TheMovieDB.MovieData
{
    public class User
    {
        //The ID
        public int id { get; set; }
        //The Email of the User
        public string email { get; set; }
        //The username (cannot be null, and is Unique)
        public string username { get; set; }
        //The password (we'll need to hide this)
        public string password { get; set; }

        public User()
        {

        }

    }
}