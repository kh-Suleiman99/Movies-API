using MoviesApi.Entities;
using System.ComponentModel.DataAnnotations;

namespace MoviesApi.Dtos
{
    public class MovieDto
    {
        [MaxLength(100)]
        public string Title { get; set; }
        public int Year { get; set; }
        public double Rate { get; set; }
        [MaxLength(250)]
        public string StoryLine { get; set; }
        public IFormFile? Poster { get; set; }
        [MaxLength(100)]
        public string Author { get; set; }
        public byte GenreId { get; set; }

    }
}