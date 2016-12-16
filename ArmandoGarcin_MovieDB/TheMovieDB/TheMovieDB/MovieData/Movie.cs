using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TheMovieDB.MovieData
{
    public class Movie
    {
        //Database Columns
        //Our ID (Primary Key)
        [Required]
        [Key]
        public int movieID { get; set; }
        //The name of the Movie
        [Required]
        [Display(Name = "Name")]
        public string movieName { get; set; }
        //The Genre ID
        public int movieGenreID { get; set; }
        //The Genre of the movie
        [Display(Name ="Genre")]
        public virtual Genre movieGenre { get; set; }
        //The Date of release
        [DataType(DataType.DateTime)]
        [Display(Name ="Release Date")]
        public DateTime? releaseDate { get; set; }

        public Movie()
        {

        }
    }
}