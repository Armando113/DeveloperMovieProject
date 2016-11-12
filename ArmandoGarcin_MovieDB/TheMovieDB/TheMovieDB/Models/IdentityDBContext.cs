using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace TheMovieDB.Models
{
    public class IdentityDBContext : IdentityDbContext<AppUser>
    {
        public IdentityDBContext() : base("IdentityString")
        {

        }
    }
}