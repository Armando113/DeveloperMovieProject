using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TheMovieDB.MovieData
{
    public class Genre
    {
        //Columns of our Database
        //Genre ID
        public int GenreID { get; set; }
        //The Genre name
        [Required]
        [Display(Name = "Genre")]
        public string GenreName { get; set; }
        //Our linked movies
        public virtual ICollection<Movie> Movies { get; set; }

        public Genre()
        {
            Movies = new List<Movie>();
        }
    }
}