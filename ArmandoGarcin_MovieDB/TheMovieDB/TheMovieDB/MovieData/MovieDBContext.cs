using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using TheMovieDB.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace TheMovieDB.MovieData
{
    public class MovieDBContext : IdentityDbContext<Models.AppUser>
    {

        public MovieDBContext() : base("TheMovieDB")
        {

        }

        //The tables in this Database
        //Our Table for the movies
        public DbSet<Movie> Movies { get; set; }
        //Our table for the Genre
        public DbSet<Genre> Genres { get; set; }
        //The Table for the users and Login
        public DbSet<User> Users { get; set; }

    }
}