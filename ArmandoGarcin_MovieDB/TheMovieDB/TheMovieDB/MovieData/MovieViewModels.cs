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

    public class ViewMovieModel
    {
        [Required]
        public int MovieGenreID { get; set; }

        [Required]
        public List<Movie> MovieList { get; set; }
    }


    public class AddMovieModel
    {
        [Required]
        [Display(Name ="Movie Name")]
        public string MovieName { get; set; }

        [Required]
        public int MovieGenreID { get; set; }

        [Required]
        [Display(Name ="Movie Genre")]
        public string MovieGenreName { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name ="Release Date")]
        public DateTime ReleaseDate { get; set; }
    }

    public class EditMovieModel
    {
        [Required]
        public int MovieID { get; set; }

        [Required]
        [Display(Name = "Movie Name")]
        public string MovieName { get; set; }

        [Required]
        public int MovieGenreID { get; set; }

        [Required]
        [Display(Name = "Movie Genre")]
        public string MovieGenreName { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Release Date")]
        public DateTime ReleaseDate { get; set; }
    }



}