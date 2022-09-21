namespace Movies_Api.Services
{
    public class GenresService : IGenresService
    {
        private readonly AppDbContext _context;
        public GenresService(AppDbContext context)
        {
            this._context = context;
        }

        public async Task<IEnumerable<Genre>> GetGenres()
        {
            return await _context.Genres.OrderBy(g => g.Name).ToListAsync();
        }
        public async Task<Genre> GetGenreById(byte id)
        {
            return await _context.Genres.SingleOrDefaultAsync(g => g.Id == id);
        }
        public async Task<bool> isValidGenre(byte id)
        {
            return await _context.Genres.AnyAsync(g => g.Id == id);
        }
        public async Task<Genre> Add(Genre genre)
        {
            await _context.Genres.AddAsync(genre);
            _context.SaveChanges();
            return genre;
        } 
        public Genre Update(Genre genre)
        {
            _context.Update(genre);
            _context.SaveChanges();
            return genre;
        }
        public Genre Delete(Genre genre)
        {
            _context.Remove(genre);
            _context.SaveChanges();
            return genre;
        }
    }
}