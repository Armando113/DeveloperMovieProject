using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using TheMovieDB.Models;

namespace TheMovieDB.MovieData
{
    public class MovieDBContext : DbContext
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