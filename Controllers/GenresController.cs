using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Data;
using MoviesApi.Dtos;
using MoviesApi.Entities;
using MoviesApi.Services;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class GenresController(IGenreServices genreServices) : ControllerBase
    {
        private readonly IGenreServices _genreServices = genreServices;

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllAsync()
        {
            var genres = await _genreServices.GetAll();
            return Ok(genres);
        }

        [HttpPost]
        public async Task<IActionResult> CreateGenreAsync(GenreDto genreDto)
        {
            var genre = new Genre { Name = genreDto.Name };
            await _genreServices.CreateGenre(genre);

            return Ok(genre);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGenreAsync(byte id, GenreDto genreDto)
        {
            var genre = await _genreServices.GetById(id);
            
            if (genre is null)
                return NotFound($"There is not genere with id = {id}");

            genre.Name = genreDto.Name;
            _genreServices.UpdateGenre(genre);

            return Ok(genre);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGenreAsync(byte id)
        {
            var genre = await _genreServices.GetById(id);

            if (genre is null)
                return NotFound($"There is not genere with id = {id}");

            _genreServices.DeleteGenre(genre);

            return Ok(genre);

        }
    }
}