namespace Movies_Api.Services
{
    public interface IMoviesService
    {
        Task<IEnumerable<Movie>> GetMovies(byte genreId=0);
        Task<Movie> GetMovieById(int id);
        Task<Movie> Add(Movie movie);
        Movie Update(Movie movie);
        Movie Delete(Movie movie);
    }
}
