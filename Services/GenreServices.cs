using Microsoft.EntityFrameworkCore;
using MoviesApi.Data;
using MoviesApi.Dtos;
using MoviesApi.Entities;

namespace MoviesApi.Services
{
    public class GenreServices(ApplicationDbContext applicationDbContext) : IGenreServices
    {
        private readonly ApplicationDbContext _dbContext = applicationDbContext;

        public async Task<IEnumerable<Genre>> GetAll()
        {
            return await _dbContext.Genres.OrderBy(g => g.Name).ToListAsync();
        }

        public async Task<Genre> GetById(byte id)
        {
            return await _dbContext.Genres.FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<Genre> CreateGenre(Genre genre)
        {
            await _dbContext.Genres.AddAsync(genre);
            await _dbContext.SaveChangesAsync();

            return genre;
        }

        public Genre UpdateGenre(Genre genre)
        {
            _dbContext.Genres.Update(genre);
            _dbContext.SaveChanges();

            return genre;
        }

        public Genre DeleteGenre(Genre genre)
        {
            _dbContext.Genres.Remove(genre);
            _dbContext.SaveChanges();

            return genre;
        }

        public async Task<bool> IsValiedGenre(byte id)
        {
            return await _dbContext.Genres.AnyAsync(g => g.Id == id);
        }
    }
}
