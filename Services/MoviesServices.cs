using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Data;
using MoviesApi.Entities;
using System.Security.Claims;

namespace MoviesApi.Services
{
    public class MoviesServices(UserManager<ApplicationUser> userManager, ApplicationDbContext applicationDbContext) : IMoviesServices
    {
        private readonly ApplicationDbContext _dbContext = applicationDbContext;

        public async Task<IEnumerable<Movie>> GetAll(byte genreId = 0)
        {
            return await _dbContext.Movies.Include(m => m.Genre)
                .Where(m => m.GenreId == genreId || genreId == 0)
                .OrderByDescending(m => m.Rate)
                .ToListAsync();
        }

        public async Task<Movie> GetById(int id)
        {
            return await _dbContext.Movies.Include(m => m.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Movie> CreateMovie(Movie movie)
        {
            await _dbContext.Movies.AddAsync(movie);
            _dbContext.SaveChanges();

            return movie;
        }
        
        public Movie UpdateMovie(Movie movie)
        {
            _dbContext.Update(movie);
            _dbContext.SaveChanges();

            return movie;
        }

        public Movie DeleteMovie(Movie movie)
        {
            _dbContext.Remove(movie);
            _dbContext.SaveChanges();

            return movie;
        }

        public async Task<string> AddFavoiteMovie(String userid, Movie movie)
        {
            var user = await userManager.Users
                .Include(u => u.FavoriteMovies)
                .FirstOrDefaultAsync(u=> u.Id == userid);

            if (user is null)
                return "There isn't user with this id";

            if(user.FavoriteMovies.Contains(movie))
                return "Movie is already added";

            user.FavoriteMovies.Add(movie);
            _dbContext.SaveChanges();
            return string.Empty;
        }

        public async Task<IEnumerable<Movie>> GetFavoiteMovies(string uid)
        {
            var user = await userManager.Users
                .Include(u => u.FavoriteMovies).ThenInclude(m=>m.Genre)
                .FirstOrDefaultAsync(u=>u.Id == uid);

            return user.FavoriteMovies;
        }

    }
}
