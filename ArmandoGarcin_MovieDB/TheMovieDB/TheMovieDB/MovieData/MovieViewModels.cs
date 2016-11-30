using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TheMovieDB.MovieData
{
    public class AddGenreModel
    {
        [Required]
        [Display(Name = "Genre")]
        public string GenreName { get; set; }
    }

    public class DeleteGenreModel
    {
        [Required]
        [Display(Name = "Genre")]
        public string GenreName { get; set; }

        [Required]
        public int GenreID { get; set; }

        [Required]
        public ICollection<Movie> MovieList { get; set; }
    }

}