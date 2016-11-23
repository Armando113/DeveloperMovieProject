using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using TheMovieDB.MovieData;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace TheMovieDB.Models
{
    public class IdentityDBContext : IdentityDbContext<AppUser>
    {
        public IdentityDBContext() : base("IdentityString")
        {
            //Always update the database to the latest version available
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<IdentityDBContext, TheMovieDB.Migrations.Configuration>());
        }

        public static IdentityDBContext Create()
        {
            return new IdentityDBContext();
        }

        //The tables in this Database
        //Our Table for the movies
        public DbSet<Movie> Movies { get; set; }
        //Our table for the Genre
        public DbSet<Genre> Genres { get; set; }

    }
}