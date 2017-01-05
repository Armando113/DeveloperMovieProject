using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TheMovieDB.MovieData
{

    public class ViewGenreModel
    {
        [Display(Name = "New Genres")]
        public List<Genre> latestGenres;

        [Display(Name = "Genre List")]
        public List<Genre> genreList;
    }

    public class AddGenreModel
    {
        [Required]
        [Display(Name = "Genre")]
        public string genreName { get; set; }
    }

    public class DeleteGenreModel
    {
        [Required]
        [Display(Name = "Genre")]
        public string genreName { get; set; }

        [Required]
        public int genreID { get; set; }

        [Required]
        public ICollection<Movie> movieList { get; set; }
    }

    public class ViewMovieModel
    {
        [Required]
        public int movieGenreID { get; set; }

        [Required]
        public List<Movie> movieList { get; set; }
    }


    public class AddMovieModel
    {
        [Required]
        [Display(Name ="Movie Name")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        public string movieName { get; set; }

        [Required]
        public int movieGenreID { get; set; }

        [Required]
        [Display(Name ="Movie Genre")]
        public string movieGenreName { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name ="Release Date")]
        public DateTime releaseDate { get; set; }
    }

    public class EditMovieModel
    {
        [Required]
        public int movieID { get; set; }

        [Required]
        [Display(Name = "Movie Name")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        public string movieName { get; set; }

        [Required]
        public int movieGenreID { get; set; }

        [Required]
        [Display(Name = "Movie Genre")]
        public string movieGenreName { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Release Date")]
        public DateTime releaseDate { get; set; }
    }



}