namespace MoviesApp.Services {
    using Microsoft.EntityFrameworkCore;
    using MoviesApp.Data;
    using MoviesApp.Data.Models;
    using MoviesApp.Services.Interfaces;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class MoviesService : IMoviesService {
        private readonly AppDbContext _context;

        public MoviesService(AppDbContext context) {
            _context = context;
        }

        public async Task AddMovieAsync(Movie movie) {
            _context.Add(movie);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteMovieAsync(int id) {
            var movie = await _context.Movies.FindAsync(id);
            if (movie != null) {
                _context.Movies.Remove(movie);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Movie>> GetAllMoviesAsync() {
            return await _context.Movies.ToListAsync();
        }

        public async Task<Movie?> GetMovieByIdAsync(int id) {
            return await _context.Movies.FindAsync(id);
        }

        public async Task<bool> MovieExistsAsync(int id) {
            return await _context.Movies.AnyAsync(e => e.Id == id);
        }

        public async Task UpdateMovieAsync(Movie movie) {
            _context.Update(movie);
            await _context.SaveChangesAsync();
        }
    }
}
