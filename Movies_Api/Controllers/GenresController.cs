using Microsoft.AspNetCore.Mvc;
using Movies_Api.Services;

namespace Movies_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly IGenresService _genresService;
        public GenresController(IGenresService genresService)
        {
                this._genresService = genresService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var genres = await _genresService.GetGenres();
            return Ok(genres);
        }
        [HttpPost]
        public async Task<IActionResult> Create(GenreDto dto)
        {
            var genre = new Genre { Name = dto.Name };
            await _genresService.Add(genre);
            return Ok(genre);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(byte id,[FromBody]GenreDto dto)
        {
            var genre = await _genresService.GetGenreById(id);
            if (genre == null)
                return NotFound($"No Genre was found with id:{id}");
            
            genre.Name = dto.Name;
            _genresService.Update(genre);
            return Ok(genre);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(byte id)
        {
            var genre = await _genresService.GetGenreById(id);
            if (genre == null)
                return NotFound($"No Genre was found with id:{id}");

            _genresService.Delete(genre);
            return Ok(genre);
        }
    }
}