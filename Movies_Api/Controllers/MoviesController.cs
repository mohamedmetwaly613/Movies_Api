using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Movies_Api.Services;

namespace Movies_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMoviesService _moviesService;
        private readonly IGenresService _genresService;
        private readonly IMapper _mapper;
        public MoviesController(IMoviesService moviesService, IGenresService genresService, IMapper mapper)
        {
            _moviesService = moviesService;
            _genresService = genresService;
            _mapper = mapper;
        }

        private List<string> allowedExtenstions = new List<string> { ".jpg", ".png"};
        private long maxAllowedPosterSize = 1048576;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var movies = await _moviesService.GetMovies();
            var dtoMovie = _mapper.Map<IEnumerable<MovieDetailsDto>>(movies);
            return Ok(dtoMovie);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var movie = await _moviesService.GetMovieById(id);
            if (movie == null)
                return NotFound();

            var dtoMovie = _mapper.Map<MovieDetailsDto>(movie);
            return Ok(dtoMovie);
        }

        [HttpGet("GetMoviesByGenreId")]
        public async Task<IActionResult> GetMoviesByGenreId(byte genreId)
        {
            var isValidGenre = await _genresService.isValidGenre(genreId);
            if (!isValidGenre)
                return BadRequest("Invalid genre ID!");

            var movies = await _moviesService.GetMovies(genreId);

            int x;
            movies.TryGetNonEnumeratedCount(out x);
            if (x == 0)
                return NotFound($"No Movies was found with GenreId: {genreId}");

            var dtoMovie = _mapper.Map<IEnumerable<MovieDetailsDto>>(movies);
            return Ok(dtoMovie);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm]MovieDto dto)
        {
            if (dto.Poster == null)
                return BadRequest("Poster Is Required");

            if (!allowedExtenstions.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
                return BadRequest("Only .png and .jpg are allowed!");

            if (dto.Poster.Length > maxAllowedPosterSize)
                return BadRequest("Max allowed size for poster is 1MB!");

            var isValidGenre = await _genresService.isValidGenre(dto.GenreId);
            if (!isValidGenre)
                return BadRequest("Invalid genre ID!");

            using var dataStream = new MemoryStream();
            await dto.Poster.CopyToAsync(dataStream);

            var dtoMovie = _mapper.Map<Movie>(dto);
            dtoMovie.Poster = dataStream.ToArray();

            await _moviesService.Add(dtoMovie);
            return Ok(dtoMovie);
            //<img src="data:image\/*;base64,@Convert.ToBase64String(item.Poster)" width="100" height="100" />
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody]MovieDto dto)
        {
            var movie = await _moviesService.GetMovieById(id);

            if (movie == null)
                return NotFound($"No movie was found with ID: {id}");

            var isValidGenre = await _genresService.isValidGenre(dto.GenreId);
            if (!isValidGenre)
                return BadRequest("Invalid genre ID!");

            if(dto.Poster != null)
            {
                if (!allowedExtenstions.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
                    return BadRequest("Only .png and .jpg are allowed!");

                if (dto.Poster.Length > maxAllowedPosterSize)
                    return BadRequest("Max allowed size for poster is 1MB!");

                using var dataStream = new MemoryStream();
                await dto.Poster.CopyToAsync(dataStream);
                movie.Poster = dataStream.ToArray();
            }

            movie.Title = dto.Title;
            movie.Year = dto.Year;
            movie.Rate = dto.Rate;
            movie.Storyline = dto.Storyline;
            movie.GenreId = dto.GenreId;

            _moviesService.Update(movie);
            return Ok(movie);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var movie = await _moviesService.GetMovieById(id);

            if (movie == null)
                return NotFound($"No movie was found with ID: {id}");

            _moviesService.Delete(movie);
            return Ok(movie);
        }
    }
}