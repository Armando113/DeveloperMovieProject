using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace TheMovieDB.Models
{
    public class AppUser : IdentityUser
    {

        public string Phone { get; set; }

        public AppUser()
        {
            
        }

    }
}