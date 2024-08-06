using MoviesApi.Entities;

namespace MoviesApi.Services
{
    public interface IMoviesServices
    {
        Task<IEnumerable<Movie>> GetAll(byte genreId = 0);
        Task<Movie> GetById(int id);
        Task<Movie> CreateMovie(Movie movie);
        Movie UpdateMovie(Movie movie);
        Movie DeleteMovie(Movie movie);

        Task<string> AddFavoiteMovie(String userid, Movie movie);
        Task<IEnumerable<Movie>> GetFavoiteMovies(string uid);
    }
}
