using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Data;
using MoviesApi.Dtos;
using MoviesApi.Entities;
using MoviesApi.Services;

namespace MoviesApi.Controllers
{
    
    [Route("Api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User")]
    public class MoviesController(IMoviesServices moviesServices,
        IGenreServices genreServices,
        IMapper mapper) : ControllerBase
    {
        private readonly IMoviesServices _moviesServices = moviesServices;
        private readonly IGenreServices _genreServices = genreServices;
        private readonly IMapper _mapper = mapper;

        private List<string> _allowedEstenstions = new() { ".jpg", ".png" };
        private long _allowedSizeByByte = 10485760;

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllAsync()
        {
            var movies = await _moviesServices.GetAll();
            return Ok(movies);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var movie = await _moviesServices.GetById(id);
            if (movie is null)
            {
                return NotFound($"There isn't any movie with id = {id}");
            }
            return Ok(movie);
        }

        [HttpGet("GetByGenreId/{genreId}")]
        public async Task<IActionResult> GetByGenreIdAsync(byte genreId)
        {
            var movies = await _moviesServices.GetAll(genreId);
            return Ok(movies);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateMovieAsync([FromForm] MovieDto movieDto)
        {
            if (movieDto.Poster is null)
            {
                return BadRequest("Poster is requierd!");
            }
            if (!_allowedEstenstions.Contains(Path.GetExtension(movieDto.Poster.FileName).ToLower()))
            {
                return BadRequest("Only png and jpg images are allowed");
            }
            if(movieDto.Poster.Length > _allowedSizeByByte)
            {
                return BadRequest("Only 10MB images length are allowed");
            }

            var _valiedGenres = await _genreServices.IsValiedGenre(movieDto.GenreId);
            if(_valiedGenres is false)
            {
                return BadRequest("Invalid genre id");
            }

            using var dataStream = new MemoryStream();
            await movieDto.Poster.CopyToAsync(dataStream);

            Movie movie = _mapper.Map<Movie>(movieDto);
            movie.Poster = dataStream.ToArray();

            await _moviesServices.CreateMovie(movie);

            return Ok(movie);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAsync(int id, [FromForm] MovieDto movieDto)
        {
            var movie = await _moviesServices.GetById(id);
            if (movie is null)
            {
                return NotFound($"There isn't any movie with id = {id}");
            }

            var _isValiedGenres = await _genreServices.IsValiedGenre(movieDto.GenreId);
            if (_isValiedGenres is false)
            {
                return BadRequest("Invalid genre id");
            }

            if(movieDto.Poster is not null)
            {
                if (!_allowedEstenstions.Contains(Path.GetExtension(movieDto.Poster.FileName).ToLower()))
                {
                    return BadRequest("Only png and jpg images are allowed");
                }
                if (movieDto.Poster.Length > _allowedSizeByByte)
                {
                    return BadRequest("Only 10MB images length are allowed");
                }

                using var dataStream = new MemoryStream();
                await movieDto.Poster.CopyToAsync(dataStream);
                movie.Poster = dataStream.ToArray();
            }

            movie.Title = movieDto.Title;
            movie.Year = movieDto.Year;
            movie.Author = movieDto.Author;
            movie.StoryLine = movieDto.StoryLine;
            movie.Rate = movieDto.Rate;
            movie.GenreId = movieDto.GenreId;

            _moviesServices.UpdateMovie(movie);
            return Ok(movie);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var movie = await _moviesServices.GetById(id);
            if (movie is null)
            {
                return NotFound($"There isn't any movie with id = {id}");
            }

            _moviesServices.DeleteMovie(movie);

            return Ok(movie);
        }

        [HttpPost("AddFavoiteMovie/{movieId}")]
        public async Task<IActionResult> AddFavoiteMoviesAsync(int movieId)
        {
            var movie = await _moviesServices.GetById(movieId);
            if (movie is null)
                return NotFound($"There isn't any movie with id = {movieId}");

            var userId = HttpContext.User.FindFirst("uid")?.Value;
            if (userId is null)
                return NotFound($"Something Went Wrong");

            var result = await _moviesServices.AddFavoiteMovie(userId, movie);
            if(!string.IsNullOrEmpty(result))
                return BadRequest(result);

            return Ok(movie);
        }

        [HttpGet("GetFavoiteMovies")]
        public async Task<IActionResult> GetFavoiteMoviesAsync()
        {
            var userId = HttpContext.User.FindFirst("uid")?.Value;
            if (userId is null)
                return NotFound($"Something Went Wrong");
            var movies = await _moviesServices.GetFavoiteMovies(userId);
            return Ok(movies);
        }

    }
}
