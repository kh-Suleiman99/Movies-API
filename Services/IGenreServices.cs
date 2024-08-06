using MoviesApi.Entities;

namespace MoviesApi.Services
{
    public interface IGenreServices
    {
        Task<IEnumerable<Genre>> GetAll();
        Task<Genre> GetById(byte id);
        Task<Genre> CreateGenre(Genre genre);
        Genre UpdateGenre(Genre genre);
        Genre DeleteGenre(Genre genre);
        Task<bool> IsValiedGenre(byte id);
    }
}
