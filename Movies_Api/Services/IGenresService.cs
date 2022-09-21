namespace Movies_Api.Services
{
    public interface IGenresService
    {
        Task<IEnumerable<Genre>> GetGenres();
        Task<Genre> GetGenreById(byte id);
        Task<bool> isValidGenre(byte id);
        Task<Genre> Add(Genre genre);
        Genre Update(Genre genre);
        Genre Delete(Genre genre);
    }
}
