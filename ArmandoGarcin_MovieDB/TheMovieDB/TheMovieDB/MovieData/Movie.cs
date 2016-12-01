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
        public int MovieID { get; set; }
        //The name of the Movie
        [Required]
        public string MovieName { get; set; }
        //The Genre ID
        public int? MovieGenreID { get; set; }
        //The Genre of the movie
        [Display(Name ="Genre")]
        public virtual Genre MovieGenre { get; set; }
        //The Date of release
        [DataType(DataType.DateTime)]
        [Display(Name ="Release Date")]
        public DateTime? ReleaseDate { get; set; }

        public Movie()
        {

        }
    }
}