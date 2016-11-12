using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TheMovieDB.MovieData
{
    public class User
    {
        //The ID
        public int ID { get; set; }
        //The Email of the User
        public string Email { get; set; }
        //The username (cannot be null, and is Unique)
        public string Username { get; set; }
        //The password (we'll need to hide this)
        public string Password { get; set; }

        public User()
        {

        }

    }
}