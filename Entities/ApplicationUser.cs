using Microsoft.AspNetCore.Identity;

namespace MoviesApi.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public ICollection<Movie> FavoriteMovies { get; set; } = new List<Movie>();

    }
}
