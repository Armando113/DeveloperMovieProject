using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using TheMovieDB.MovieData;

namespace TheMovieDB.Models
{
    public class AppUserStore : UserStore<AppUser>
    {

        public AppUserStore(MovieDBContext _context) : base(_context)
        {

        }

    }
}